using System;
using System.Linq;
using UnityEngine;

namespace ONICPU.digit
{
  public class DigitSeg : KMonoBehaviour, ISaveLoadable
  {
    private LogicPorts ports;

    public static readonly HashedString READ_PORT_ID = new HashedString("LogicRead");

    public static readonly KAnimHashedString SegNegId = new KAnimHashedString("seg_neg");
    private static readonly KAnimHashedString[][] SegId = new KAnimHashedString[7][];
    private const int MAX_SEG_VALUE = 16;

    public int displayBits = 0;

    private KBatchedAnimController kbac;
    private int value = 0;

    static DigitSeg()
    {
      for (int i = 0; i < SegId.Length; i++)
      {
        KAnimHashedString[] arr = new KAnimHashedString[10];
        for (int j = 0; j < arr.Length; j++)
          arr[j] = new KAnimHashedString($"seg{j}_{i}");
        SegId[i] = arr;
      }
    }
   
    private Color activeTintColor = new Color(1f, 0.4f, 0.4f);
    private Color inactiveTintColor = new Color(40 / 255.0f, 58 / 255.0f, 46 / 255.0f);

    private static readonly EventSystem.IntraObjectHandler<DigitSeg> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<DigitSeg>(delegate (DigitSeg component, object data)
    {
      component.OnLogicValueChanged(data);
    });
    protected override void OnSpawn()
    {
      Subscribe(-801688580, OnLogicValueChangedDelegate);
      ports = GetComponent<LogicPorts>();
      kbac = GetComponent<KBatchedAnimController>();
      kbac.Play("on");
    }
    protected override void OnCleanUp()
    {
      Unsubscribe(-801688580, OnLogicValueChangedDelegate);
      base.OnCleanUp();
    }

    public void OnLogicValueChanged(object data)
    {
      var logicValueChanged = (LogicValueChanged)data;
      if (logicValueChanged.portID == READ_PORT_ID) 
      {
        value = ports.GetInputValue(READ_PORT_ID);
        RefreshAnim();
      }
    }


    private void DisplayOverflow(int maxCount)
    {
      Update7Seg(kbac, 1, 0);
      Update7Seg(kbac, 0, 15);
      for (short i = 2; i < maxCount; i++)
        Update7Seg(kbac, i, MAX_SEG_VALUE);
    }
    private void RefreshAnim()
    {
      switch(displayBits)
      {
        case 8:
        case 16:
          {
            var maxCount = (displayBits == 8 ? 3 : 5);
            if (displayBits == 8)
            {
              if (value > 999 || value < -999)
              {
                //of
                DisplayOverflow(maxCount);
                return;
              }
            }
            else
            {
              if (value > short.MaxValue || value < short.MinValue)
              {
                //of
                DisplayOverflow(maxCount);
                return;
              }
            }

            short number1 = (short)value; //8bit char; 16 bit short
            short number = (short)Math.Abs((int)number1);
            short index = 0;

            if (number1 == 0)
            {
              short number2 = 0;
              for (; index < maxCount; index++)
              {
                Update7Seg(kbac, index, number2);
                number2 = MAX_SEG_VALUE;
              }
              Update7Seg(kbac, -1, 1);
            }
            else
            {
              while (number > 0)
              {
                Update7Seg(kbac, index, (short)(number % 10));
                number = (short)(number / 10);
                index++;
              }
              if (number1 < 0)
              {
                Update7Seg(kbac, index >= maxCount ? (short)-1 : index, -1);
                index++;
              }
              else
              {
                Update7Seg(kbac, -1, 1);
              }
              for (; index < maxCount; index++)
                Update7Seg(kbac, index, MAX_SEG_VALUE);
            }
            break;
          }
        case 32:
          {
            const int maxCount = 10;
            var number1 = (long)value;
            if (number1 > int.MaxValue || number1 < int.MinValue)
            {
              //of
              DisplayOverflow(maxCount);
              return;
            }
            var number = Math.Abs(number1);
            short index = 0;
            if (number1 == 0)
            {
              short number2 = 0;
              for (; index < maxCount; index++)
              {
                Update7Seg(kbac, index, number2);
                number2 = MAX_SEG_VALUE;
              }
              Update7Seg(kbac, -1, 1);
            }
            else
            {
              while (number > 0)
              {
                Update7Seg(kbac, index, (short)(number % 10));
                number = number / 10;
                index++;
              }
              if (number1 < 0)
              {
                Update7Seg(kbac, index >= maxCount ? (short)-1 : index, -1);
                index++;
              }
              else
              {
                Update7Seg(kbac, -1, 1);
              }
              for (; index < maxCount; index++)
                Update7Seg(kbac, index, MAX_SEG_VALUE);
            }
            break;
          }
      }
    }

    private static readonly short[][] Seg7ActiveMap = new short[7][]
    {
      new short[] { 2,3,5,6,7,8,9,0,10,12,14,15},     //a
      new short[] { 1,2,3,4,7,8,9,0,10,13 },          //b
      new short[] { 1,3,4,5,6,7,8,9,0,10,11,13 },     //c
      new short[] { 2,3,5,6,8,9,0,11,12,13,14},       //d
      new short[] { 2,6,0,8,10,11,12,13,14,15},       //e
      new short[] { 4,5,6,8,9,0,10,11,12,14,15 },     //f
      new short[] { 2,3,4,5,6,8,9,10,11,12,13,14,15 },//g
    };

    private void Update7Seg(KBatchedAnimController component, short index, short value)
    {
      if (index == -1)
      {
        Utils.TintSymbolConditionally(true, value < 0, component, SegNegId, activeTintColor, inactiveTintColor);
      }
      else
      {
        if (value >= 0)
        {
          for (int i = 0; i < Seg7ActiveMap.Length; i++)
            Utils.TintSymbolConditionally(true, Seg7ActiveMap[i].Contains(value), component, SegId[i][index], activeTintColor, inactiveTintColor);
        }
        else
        {
          for (int i = 0; i < Seg7ActiveMap.Length - 1; i++)
            Utils.TintSymbolConditionally(true, false, component, SegId[i][index], activeTintColor, inactiveTintColor);
          Utils.TintSymbolConditionally(true, true, component, SegId[6][index], activeTintColor, inactiveTintColor);
        }
      }
    }
  }
}
