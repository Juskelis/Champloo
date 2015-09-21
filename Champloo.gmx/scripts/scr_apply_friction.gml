///scr_apply_friction(current_force, friction)
//argument0 current force
//argument1 current friction
//returns applied friction to force

return scr_approach(argument0, 0, argument1);

/*
var appliedforce = argument0 - sign(argument0)*argument1;
if(sign(appliedforce) != sign(argument0))
    return 0;
return appliedforce;
*/
