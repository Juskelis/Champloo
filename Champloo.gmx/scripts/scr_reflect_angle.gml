//argument0: angle we are reflecting over
//argument1: angle we are going to reflect

var ref = (2*argument0 - argument1)%360;
if(ref < 0)
{
    ref += 360;
}

return ref;
