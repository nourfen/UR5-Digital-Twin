using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.MixedReality.WebRTC;
using UnityEngine;
using Microsoft.MixedReality.WebRTC.Unity;
using PeerConnection = Microsoft.MixedReality.WebRTC.PeerConnection;

public class CameraVideoStream : MonoBehaviour
{
    public int DefaultPort = 9998;
    private readonly IPAddress _signalingServerIpAddress = IPAddress.Any;
    private DataChannel _dataChannel;
    private WebRTCSignaling _signaling;
    private PeerConnection _serverPeer = new PeerConnection();
    
    private VideoRenderer _videoRenderer;
    private RemoteVideoTrack _track;
    async void Start()
    {

        _videoRenderer = GetComponent<VideoRenderer>();
        var config = new PeerConnectionConfiguration
        {
            IceServers = new List<IceServer> {
                new IceServer{ Urls = { "stun:stun.l.google.com:19302" } }
            }
        };
        //Configuring Peer Connection:
		    
        if (!_serverPeer.Initialized) await _serverPeer.InitializeAsync(config);
        Console.WriteLine("Client Peer Connection is Initialized");
		    
        
        //Signaling Task:
        _signaling = new WebRTCSignaling(_serverPeer);
        _dataChannel = await _serverPeer.AddDataChannelAsync("Control Channel", true, true);
        
        RegisterSignalingEventsHandler();  
        
        Task connectTask = null;
        connectTask = _signaling.Listen(_signalingServerIpAddress, DefaultPort);
        Debug.Log("Connecting to remote peer...");
        await connectTask.ConfigureAwait(false);
        Debug.Log("Connected!");
    }

    void OnApplicationQuit()
    {
        Dispose();
    }

    private void RegisterSignalingEventsHandler()
    {
        _signaling.Connected += async () =>  Debug.Log("Signaling Connected");
        _signaling.Disconnected += () => UnityEngine.Debug.Log("Signaling Disconnected");

        _signaling.SdpMessageReceived += async (SdpMessage message) =>
        {
            await _serverPeer.SetRemoteDescriptionAsync(message);
            if (message.Type == SdpMessageType.Offer)
            {
                _serverPeer.CreateAnswer();
            }
        };
		    
        _signaling.IceCandidateReceived += (IceCandidate candidate) => {
            _serverPeer.AddIceCandidate(candidate);
        };
		    
        _serverPeer.Connected += () => {
            Debug.Log("PeerConnection: connected.");
            //_signaling.Stop = true;
        };

        _serverPeer.IceStateChanged += (IceConnectionState newState) => {
            Debug.Log($"ICE state: {newState}");
        };
        
        _serverPeer.VideoTrackAdded += (RemoteVideoTrack track) =>
        {
            _track = track;
            _videoRenderer.StartRendering(_track);
        };

        _dataChannel.MessageReceived += (bytes) =>
        {
            string message = Encoding.Default.GetString(bytes);
            Debug.Log("Message Received: " + message);
        };
    }
    
    private void Dispose()
    {
        Debug.Log("DISPOSE");
        //_serverPeer.Dispose();
        _videoRenderer.StopRendering(_track);
        _signaling.Dispose();
        
    }
}
