//argument1 current force
//argument2 current friction
//returns applied friction to force
var appliedforce = argument0 - sign(argument0)*argument1;
if(sign(appliedforce) != sign(argument0))
    return 0;
return appliedforce;
