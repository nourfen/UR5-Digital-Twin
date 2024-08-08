using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.MixedReality.WebRTC;
using Debug = UnityEngine.Debug;

public class WebRTCSignaling
{
    private TcpListener _tcpListener;
    private TcpClient _client;
    
    private byte[] _msgBuffer = new byte[1024];

    public event Func<Task> Connected;
    
    #region Events
    
    public PeerConnection.LocalSdpReadyToSendDelegate SdpMessageReceived;
    public PeerConnection.IceCandidateReadytoSendDelegate IceCandidateReceived;

    #endregion
    
    private Task _connectAndReceiveTask;
    private Task _connectAndReceiveTask2;
    protected async Task OnConnected()
    {
        try
        {
            if (Connected != null)
            {
                await Connected.Invoke();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    
    public PeerConnection PeerConnection { get; }
    public WebRTCSignaling(PeerConnection pc)
    {
        PeerConnection = pc;
	        
        PeerConnection.DataChannelAdded += PeerConnection_DataChannelAdded;
        PeerConnection.DataChannelRemoved += PeerConnection_DataChannelRemoved;
        PeerConnection.LocalSdpReadytoSend += PeerConnection_LocalSdpReadytoSend;
        PeerConnection.IceCandidateReadytoSend += PeerConnection_IceCandidateReadytoSend;
    }
    
    public async Task Listen(IPAddress ipAddress, int port)
    {
	    _connectAndReceiveTask = Task.Run(async () =>
	    {

		    _connectAndReceiveTask2 = ListenAsync(ipAddress, port);
		    // NOTE: called after first await in _connectAndReceiveTask2 task

		    await _connectAndReceiveTask2.ConfigureAwait(false);
	    });
	    await _connectAndReceiveTask.ConfigureAwait(false);
    }

    public async Task ListenAsync(IPAddress ipAddress, int port)
    {
        Debug.Assert(_tcpListener == null);
        _tcpListener = new TcpListener(ipAddress, port);
        //Starts listening for incoming connection requests with a maximum number of pending connection.
        _tcpListener.Start(1);
        
        Debug.Log("==========> Waiting for a connection. <==========");
        var accept = _tcpListener.AcceptTcpClientAsync();

        _client = await accept;

        Debug.Log("==========> Signaling Connected! <==========");

        await OnConnected();

        try
        {
            await ReceiveLoopAsync()/*.ConfigureAwait(false)*/;
        }
        finally
        {
            Debug.Log("ReceiveLoopAsync exited");
            Debug.Log("==========> Signaling Disconnecting! <==========");
            OnDisconnected();
            Debug.Log("==========> Signaling Disconnected! <==========");
        }
    }
    
    private async Task ReceiveLoopAsync()
    {
	    var stream = _client.GetStream();
			
	    while (true)
	    { 
		    Debug.Log("===== Receive Start =====");
		    byte[] lengthBuffer = new byte[sizeof(int)];
		    int numberOfBytesRead = 0;
				
		    //Length of the message
		    numberOfBytesRead = await stream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length)/*.ConfigureAwait(false)*/;
				
		    // Note: await is a suspension point, which means that it is possible that socket is disposed or cancellation is requested while being suspended causing ReadAsync to return 0
		    if (numberOfBytesRead == 0)
		    {
			    Debug.Log("Read 0 bytes; exiting receive loop");
			    return;
		    }
		    Debug.Assert(numberOfBytesRead == lengthBuffer.Length, "numberOfBytesRead != lengthBuffer.Length");
				
		    //Flip it back
		    int msgLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lengthBuffer, 0));
				
		    //Message to be received
		    byte[] msgBuffer = new byte[msgLength];

		    // do multiple read passes if the message couldn't be read with one read
		    const int MAX_TRIES = 3;
		    int tries = MAX_TRIES;
		    numberOfBytesRead = 0;
		    int numberOfBytesReadSum = 0;
		    do
		    { 
			    numberOfBytesRead = await stream.ReadAsync(msgBuffer, numberOfBytesReadSum, msgLength - numberOfBytesReadSum)/*.ConfigureAwait(false)*/;
			    
			    numberOfBytesReadSum += numberOfBytesRead;
			    Debug.Log($"ReadAsync: {numberOfBytesReadSum}/{msgLength}");

			    // Note: await is a suspension point, which means that it is possible that socket is disposed or cancellation is requested while being suspended causing ReadAsync to return 0
			    if (numberOfBytesRead == 0)
			    { 
				    Debug.Log("Read 0 bytes; exiting receive loop");
				    return;
			    }
			    
			    tries--;
		    } while (numberOfBytesReadSum < msgLength && tries > 0);

		    // if (tries == 0)
		    // {
		    // 	Console.WriteLine("+++> Need more than " + MAX_TRIES + "passes to read message. Pulling the plug, the message is too big.");
		    // 	// TODO: return a value or exception so that the server can react e.g. by restarting
		    // 	Console.WriteLine("TODO: return a value or exception so that the endpoint can react e.g. by restarting");
		    // 	if (Debugger.IsAttached) Debugger.Break();
		    // 	return;
		    // }

		    var msg = new UTF8Encoding(false, true).GetString(msgBuffer, 0, msgLength);
		    //Debug.Log("Message: " + msg);

		    ParseMessage(msg);
	    }
	}
    
    private void ParseMessage(string msg)
    {
	    
	    string[] lines = msg.Split('\n');
	    string line = lines[0];
	    UnityEngine.Debug.Log($"[<-] {lines}");
	    if (line == "ice")
	    {
		    string sdpMid = lines[1];
		    int sdpMlineindex = int.Parse(lines[2]);

		    // The ICE candidate is a multi-line field, ends with an empty line
		    string candidate = "";
		    for (int i = 3; i < lines.Length; i++)
		    {
			    line = lines[i];
			    if (line.Length == 0) break;
			    candidate += line;
			    candidate += "\n";
		    }

		    Debug.Log($"[<-] ICE candidate: {sdpMid} {sdpMlineindex} {candidate}");
		    var iceCandidate = new IceCandidate
		    {
			    SdpMid = sdpMid,
			    SdpMlineIndex = sdpMlineindex,
			    Content = candidate
		    };
		    IceCandidateReceived?.Invoke(iceCandidate);
	    } else if (line == "sdp")
	    {
		    string type = lines[1];

		    // The SDP message content is a multi-line field, ends with an empty line
		    string sdp = "";
		    for (int i = 2; i < lines.Length; i++)
		    {
			    line = lines[i];
			    if (line.Length == 0) break;
			    sdp += line;
			    sdp += "\n";
		    }

		    Debug.Log($"[<-] SDP message: {type} {sdp}");
		    var message = new SdpMessage { Type = SdpMessage.StringToType(type), Content = sdp };
		    SdpMessageReceived?.Invoke(message);
	    }
    
		//Debug.Log("Finished processing messages");
    }
        
    public event Action Disconnected;
    protected void OnDisconnected()
    {
        try
        {
            Disconnected?.Invoke();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    
    private void PeerConnection_IceCandidateReadytoSend(IceCandidate candidate)
    {
        // See ProcessIncomingMessages() for the message format
        SendMessage($"ice\n{candidate.SdpMid}\n{candidate.SdpMlineIndex}\n{candidate.Content}\n\n");
    }
    private void PeerConnection_LocalSdpReadytoSend(SdpMessage message)
    {
        // See ProcessIncomingMessages() for the message format
        string typeStr = SdpMessage.TypeToString(message.Type);
        SendMessage($"sdp\n{typeStr}\n{message.Content}\n\n");
    }
    private void PeerConnection_DataChannelAdded(DataChannel channel)
    {
        Console.WriteLine($"Event: DataChannel Added {channel.Label}");
        channel.StateChanged += () => { Console.WriteLine($"DataChannel '{channel.Label}':  StateChanged '{channel.State}'"); };
    }
	    
    private void PeerConnection_DataChannelRemoved(DataChannel channel)
    {
        Console.WriteLine($"Event: DataChannel Removed {channel.Label}");
    }
    public async void SendMessage(string message)
    {
	    byte[] msgBuffer = new byte[sizeof(int) + message.Length];
	    int length = message.Length;
	    byte[] payload = Encoding.Default.GetBytes(message);
	    //Debug.Log("Byte Array is: " + String.Join(" ", payload));
	    
	    // var msg = Encoding.Default.GetString(payload);
	    // Debug.Log("Message: " + msg);
	    // prepend length in front of array
	    int networkLength = IPAddress.HostToNetworkOrder(length);
	    byte[] encodedMessageLength = BitConverter.GetBytes(networkLength);
	    encodedMessageLength.CopyTo(msgBuffer, 0);
	    payload.CopyTo(msgBuffer, sizeof(int));
	    await _client?.GetStream()?.WriteAsync(msgBuffer, 0, msgBuffer.Length);
	    //SignalingSolution.send(msg)
    }
        
    public void Dispose()
    {
        PeerConnection.LocalSdpReadytoSend -= PeerConnection_LocalSdpReadytoSend;
        PeerConnection.DataChannelAdded -= PeerConnection_DataChannelAdded;
        PeerConnection.DataChannelRemoved -= PeerConnection_DataChannelRemoved;
        PeerConnection.IceCandidateReadytoSend -= PeerConnection_IceCandidateReadytoSend;
        
        try {
                
            _client?.Dispose();
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            
            throw;
        }

        try
        {
            _tcpListener?.Stop();

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
        _tcpListener = null;
    }
}
