using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class Base36Converter
{

    static List<string> base36Strings = new List<string> {

                                "0",
                                "1",
                                "2",
                                "3",
                                "4",
                                "5",
                                "6",
                                "7",
                                "8",
                                "9",
                                "A",
                                "B",
                                "C",
                                "D",
                                "E",
                                "F",
                                "G",
                                "H",
                                "I",
                                "J",
                                "K",
                                "L",
                                "M",
                                "N",
                                "O",
                                "P",
                                "Q",
                                "R",
                                "S",
                                "T",
                                "U",
                                "V",
                                "W",
                                "X",
                                "Y",
                                "Z"

    };

    static float baseStep = 500f;

   

    static public string EncodeFloatToBase36(float val,float baseVal,float maxVal){
        // For ease's sake, we're going to assume a total of 500 possible values for any given range
    
        
        float valStep = (((maxVal - baseVal)/baseStep) * 1000);
        float valueChange = Mathf.Clamp(val - baseVal,0,baseStep) * 1000;
        
        int stepTotal = (int)Mathf.Round(valueChange/valStep);

        return int_to_string_base36(stepTotal);    
    } 

    static public string EncodeBoolListToBase36(List<bool> boolList) {

        if(boolList.Count > 9) Debug.Log("ERROR: Can only encode up to 9 booleans");

        int bitWise = 0;

        for(var q =0; q < 9;q++)
        {
            int tempBitwise = 0;
            if(q < boolList.Count && boolList[q]) tempBitwise = 1 << q;
            else tempBitwise = 0;
            
            bitWise |= tempBitwise;
        }

        return int_to_string_base36(bitWise);
    }

    static string int_to_string_base36(int stepTotal) {

        int tens = stepTotal/36;
        int ones = stepTotal % 36;

        var encodedStr = base36Strings[tens] + base36Strings[ones];

        return encodedStr;
    }

    static int string_to_int_base36(string encodedString){

        int tens = base36Strings.FindIndex( x => x.Contains(encodedString[0]));
        int ones = base36Strings.FindIndex( x => x.Contains(encodedString[1]));

        if(tens == -1 || ones == -1) return -1;
        
        tens *= 36;

        return (tens + ones);

    }


    static public float DecodeFloatFromBase36(string encodedString,float baseVal,float maxVal) {

        int valStep = (int)(((maxVal - baseVal)/baseStep) * 1000);
        
        int totalSteps = string_to_int_base36(encodedString);

        if(totalSteps == -1) return -1;

        float decodedVal = ((valStep * totalSteps)/1000f) + baseVal;
        return decodedVal;
    }


   static public List<bool> DecodeBoolListFromBase36(string encodedStr) {

        int bitWise = string_to_int_base36(encodedStr);

        List<bool> boolList = new List<bool>();

        for(var q = 0; q < 9;q++)
        {
            int tempBitwise = 1 << q;
            int temp1 = tempBitwise & bitWise;
            bool temp2 = temp1 == tempBitwise;

            if((tempBitwise&bitWise) == tempBitwise) boolList.Add(true);
            else boolList.Add(false);
        }

        return boolList;

    }

}


