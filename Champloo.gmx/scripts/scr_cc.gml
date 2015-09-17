//checks to see if it is shorter to rotate angle A clockwise or counter-clockwise
//  towards b
//returns true if we should go counter-clockwise, false otherwise

//argument0: initial
//argument1: target
var a = argument0;
var b = argument1;

var a1 = a;
var a2 = 180 + a1;
var lr = 0;

if(a2 > 360)
{
    a2 = a2%360;
    lr = 1;
}

if(a1 < b && b < a2)
{
    if(lr == 0)
    {
        return true;
    }
    else
    {
        return false;
    }
}
else
{
    if(lr == 1)
    {
        return true;
    }
    else
    {
        return false;
    }
}
