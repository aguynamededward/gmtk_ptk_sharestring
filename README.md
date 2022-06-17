# gmtk_ptk_sharestring
_An encoder / decoder for GMTK's Platformer Toolkit._

It has two classes: **PTKdata** and **Base36Converter**.  
    
**Base36Converter** is a class for converting floats and bools to a string.  You don't need to touch it unless you want to.

**PTKdata** is where everything happens.

##How does PTKdata work?

Create an instance of the PTKdata() class - this will hold a copy of all the variables the PTK system allows you to control.

_NOTE: "Time scale" was not included, as it isn't a "tuning" variable, just a debugging one._

Keep the PTKAdata instance updated with the variables from inside the game like so:
```
            // for float values
            setData(pt_float.float_enum,floatValue)

            // for bool values
            setData(pt_bool.bool_enum,true/false)
```
You can find the full list of enums avaiable in the enum declaration, but they should be pretty self explanatory.

When you're ready to generate the sharestring, call:

    EncodePTKdata();

It'll return a string in the format:

    "GM#############(repeats for 48 charaters)#####TK"

    
Likewise, to decode a string and apply it to the data:

    DecodePTKdata(encodedString)

(DecodePTKdata does some basic string validation to make sure it can process the data, but if it runs into something it doesn't understand, it skips it)

The PTKdata object will now have all the updated values, and you can access them to copy out to the objects in the scene using

    getData(pt_float.enum);
    getData(pt_bool.enum);
    
This was coded by Edward "Words" Newton ([@aguynamededward](http://twitter.com/aguynamededward)) from the GMTK Discord, with the help of @Creta Park (also from the GTMK discord).

