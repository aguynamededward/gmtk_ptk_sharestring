using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#region README

/*
    Platform Sharer for GMTK's Platformer's toolkit

    

    It has two classes: PTKdata and Base36Converter.  
    
    Base36Converter is a class for converting floats and bools to a string.  You don't need to touch it unless you want to.


    PTKdata is where everything happens.

    How does PTKdata work?

    Create an instance of the PTKdata() class - this will hold a copy of all the variables the PTK system allows you to control.

    NOTE: "Time scale" was not included, as it isn't a "tuning" variable, just a debugging one.


    Keep the PTKAdata instance updated with the variables from inside the game like so:

            // for float values
            setData(pt_float.float_enum,floatValue)

            // for bool values
            setData(pt_bool.bool_enum,true/false)

    You can find the full list of enums avaiable in the enum declaration, but they should be pretty self explanatory.

    (getData() is also available, if you need it)


    When you're ready to generate the sharestring, call:

    EncodePTKdata();

    It'll return a string in the format:
    "GM#############(repeats for 48 charaters)#####TK"

    
    Likewise, to decode a string and apply it to the data:

    DecodePTKdata(encodedString)

    (DecodePTKdata does some basic string validation to make sure it can process the data, but if it runs into something it doesn't understand, it skips it)

    The PTKdata object will now have all the updated values, and you can copy them back out to the objects in the scene using

    getData(pt_float.enum);
    getData(pt_bool.enum);


    This was coded by Edward "Words" Newton (@aguynamededward) from the GMTK Discord, with initial variable references provided by Creta Park (also from the GTMK discord).



*/

#endregion


public class PTKdata : MonoBehaviour
{

    public class PTKFloatValue { 

        float _setting = 0f;
        public float setting {
            
            get => _setting;
            set {
                _setting = Mathf.Clamp(value,baseSetting,maxSetting);
            }
        }
        public float baseSetting = 0f;
        public float maxSetting = 100f;

        public PTKFloatValue(float val,float baseVal,float maxVal){
            
            maxSetting = maxVal;
            baseSetting = baseVal;
            setting = val;
            
        }


    }
    public PTKdata() {
            // Do nothing, it'll init the default settings
    }

        
    #region declare base variables
    
    List<bool> boolData = new List<bool>() {
                false, // jumpVariableHeight
                false,  // jumpDoubleJump
                false,  // cameraIgnoreJumps
                false,  // assistRoundedCorners
                false,  // juiceSFXJumping
                false   // juiceSFXLanding
    };
    
    public enum pt_bool {
            jumpVariableHeight,
            jumpDoubleJump,
            cameraIgnoreJumps,
            assistRoundedCorners,
            juiceSFXJumping,
            juiceSFXLanding
    }


    public List<PTKFloatValue> floatData = new List<PTKFloatValue>(){
        
        new PTKFloatValue(0f,2f,80f),       // runningAcceleration - 0
        new PTKFloatValue(0f,2f,80f),       // runningDeceleration
        new PTKFloatValue(0f,2f,25f),       // runningMaxSpeed
        new PTKFloatValue(0f,2f,5.5f),      // jumpHeight
        new PTKFloatValue(0f,1f,10f),       // jumpDownGravity
        new PTKFloatValue(0f,0.2f,1.2f),    // jumpDuration - 5
        new PTKFloatValue(0f,1f,80f),       // jumpAirControl
        new PTKFloatValue(0f,1f,80f),       // jumpAirBrake
        new PTKFloatValue(0f,4f,10f),       // cameraZoom
        new PTKFloatValue(0f,0f,2f),        // cameraDampingX
        new PTKFloatValue(0f,0f,2f),        // cameraDampingY - 10
        new PTKFloatValue(0f,0f,0.5f),      // cameraLookAhead
        new PTKFloatValue(0f,0f,0.4f),      // assistCoyoteTime
        new PTKFloatValue(0f,0f,0.4f),      // assistJumpBuffer
        new PTKFloatValue(0f,1f,20f),       // assistTerminalVelocity
        new PTKFloatValue(0f,0f,10f),       // juiceParticlesRun - 15
        new PTKFloatValue(0f,0f,20f),       // juiceParticlesJump
        new PTKFloatValue(0f,0f,50f),       // juiceParticlesLand
        new PTKFloatValue(0f,0f,1.8f),      // juiceSquashJump
        new PTKFloatValue(0f,0f,1.8f),      // juiceSquashLand
        new PTKFloatValue(0f,0f,10f),       // juiceTrail - 20
        new PTKFloatValue(0f,-20f,20f),     // juiceLeanAngle
        new PTKFloatValue(0f,10f,60f),      // juiceLeanSpeed
    };


    public enum pt_float{
        runningAcceleration,
        runningDeceleration,
        runningMaxSpeed,
        jumpHeight,
        jumpDownGravity,
        jumpDuration,
        jumpAirControl,
        jumpAirBrake,
        cameraZoom,
        cameraDampingX,
        cameraDampingY,
        cameraLookAhead,
        assistCoyoteTime,
        assistJumpBuffer,
        assistTerminalVelocity,
        juiceParticlesRun,
        juiceParticlesJump,
        juiceParticlesLand,
        juiceSquashJump,
        juiceSquashLand,
        juiceTrail,
        juiceLeanAngle,
        juiceLeanSpeed
    }

    int  encodedStringRequiredLength = 52;

    #endregion


    public void setData(pt_float en,float v){
        floatData[(int)en].setting = v;

    }

    public void setData(pt_bool en,bool b){
        boolData[(int)en] = b;
    }

    public float getData(pt_float en){
        
        return floatData[(int)en].setting;

    }

    public bool getData(pt_bool en){
        return boolData[(int)en];
    }

    public string EncodePTKData() {

        string encodedString = "";

        encodedString += "GM";
        
        foreach(PTKFloatValue p in floatData)
        {
            encodedString += EncodePTKFloatToBase36(p);
        }

        encodedString += Base36Converter.EncodeBoolListToBase36(boolData);

        encodedString += "TK";

        return encodedString;
    }


    public bool DecodePTKData(string encodedString){
        #region Validate the incoming string

        bool _fail = false;
        string _failReason = "";

        encodedString = encodedString.ToUpper();

        if(encodedString.Length != encodedStringRequiredLength || encodedString.Length != ((floatData.Count * 2) + 6)) {

            _failReason += " Encoded String is incorrect length\n";
            _fail = true;
        }

        if(!encodedString.StartsWith("GM") || !encodedString.EndsWith("TK"))
            {
                _failReason += "Encoded String is incorrectly formatted\n";
                _fail = true;
            }

        // Remove non alphanumberic characters
        string str = string.Concat(encodedString.Where(c => char.IsLetterOrDigit(c)));

        if(str.Length != encodedString.Length)
            {
                _failReason += "Encoded string contains invalid characters\n";
                _fail = true;
            }

        if(_fail) 
            {
                Debug.Log("Decoding Failed: " + _failReason);
                return false;
            }

        encodedString = str;

        #endregion


        int stringPos = 2;
        float tempFloat = 0;

        var i = 0;

        foreach(PTKFloatValue p in floatData)
            {
                
                if(encodedString.Length - 2 < stringPos) return false; // Backup in case we somehow got a too-short string

                tempFloat = Base36Converter.DecodeFloatFromBase36(encodedString.Substring(stringPos,2),p.baseSetting,p.maxSetting);
                if(tempFloat == -1)
                    {
                        Debug.Log("ERROR DECODING PTKFloat Value on #" + i);
                        return false;
                    }

                p.setting = tempFloat;
                stringPos += 2;
                i++;
            }

        if(encodedString.Length - 2 < stringPos) return false; // Backup in case we somehow got a too-short string

        List<bool> tempBoolList = Base36Converter.DecodeBoolListFromBase36(encodedString.Substring(stringPos,2));

        i = 0;
        foreach(bool b in tempBoolList)
            {
                if(i < boolData.Count) boolData[i] = b;
                else break;
            }

        return true;
    }


    // The debug implementation to make sure the encoding worked
    // private void Start() {
        
    //     string encodedData = EncodePTKData();

    //     setData(pt_float.runningAcceleration,13.5f);
    //     setData(pt_bool.cameraIgnoreJumps,true);

    //     DecodePTKData(encodedData);

    //     Debug.Log("Is the setting for runningAcceleration still 13.5? " + getData(pt_float.runningAcceleration));

    //     foreach(PTKFloatValue p in floatData){

    //         p.setting = 5f;
    //     }

    //     encodedData = EncodePTKData();

    //     setData(pt_float.runningAcceleration,13.5f);
    //     setData(pt_bool.cameraIgnoreJumps,true);

    //     DecodePTKData(encodedData);

    //     Debug.Log("Is the setting for runningAcceleration still 13.5? " + getData(pt_float.runningAcceleration));
    // }



    
    string EncodePTKFloatToBase36(PTKFloatValue data){

        return Base36Converter.EncodeFloatToBase36(data.setting,data.baseSetting,data.maxSetting);
    }

    float DecodePTKFloatFromBase36(string data,PTKFloatValue ptkFloat){

        return Base36Converter.DecodeFloatFromBase36(data,ptkFloat.baseSetting,ptkFloat.maxSetting);
    }



}
