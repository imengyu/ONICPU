using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ONICPU.wireless
{
  public class DigitBroadcastManager : MonoBehaviour
  {
    private static DigitBroadcastManager broadcastManager;

    public DigitBroadcastManager()
    {

      for (int i = 0; i < CHANNEL_COUNT; i++)
        Channels[i] = new DigitBroadcastChannel();
    }

    public const int CHANNEL_COUNT = 16;

    public static DigitBroadcastManager GetInstancee()
    {
      if (broadcastManager == null)
      {
        GameObject go = new GameObject("DigitBroadcastManager");
        broadcastManager = go.AddComponent<DigitBroadcastManager>();
      }
      return broadcastManager;
    }

    public class DigitBroadcastChannel
    {
      public int Value;
      public int RefCount;
    }

    public DigitBroadcastChannel[] Channels = new DigitBroadcastChannel[CHANNEL_COUNT];
  }
}
