using UnityEngine;

[RequireComponent(typeof(Osc))]
[RequireComponent(typeof(UDPPacketIO))]
[RequireComponent(typeof(TripodController))]
public class TripodToOSC : MonoBehaviour
{
    [SerializeField]
    private string RemoteHost = "localhost";

    [SerializeField]
    private int RemotePort = 6448;

    [SerializeField]
    private string RemoteOscAddress = "/wek/inputs";

    [SerializeField]
    private int ListenerPort = 12000;

    [SerializeField]
    private string LocalOscAddress = "/wek/outputs";

    private Osc handler;

    // Start is called before the first frame update
    void Start()
    {
        UDPPacketIO udp = (UDPPacketIO)this.GetComponent("UDPPacketIO");
        udp.init(RemoteHost, RemotePort, ListenerPort);
        
        handler = (Osc)this.GetComponent("Osc");
        handler.init(udp);

        //Tell Unity to call function Example1 when message /wek/outputs arrives
        //handler.SetAddressHandler("/wek/outputs", oscCallback);

        Debug.Log("OSC Running");


        TripodController TripodController = GetComponent<TripodController>();

        TripodController.OnTripodViewUpdate += OnTripodViewUpdate;
    }

    private void OnTripodViewUpdate(Texture2D texture)
    {
        Color[] colors = texture.GetPixels(0, 0, texture.width, texture.height);
        OscMessage message = new OscMessage();
        message.Address = RemoteOscAddress;

        foreach(Color color in colors)
        {
            message.Values.Add(color.r);
            message.Values.Add(color.g);
            message.Values.Add(color.b);
        }

        Debug.Log("Sending osc message with length: " + message.Values.Count + " to " + RemoteHost + ":" + RemotePort);
        handler.Send(message);
    }
}
