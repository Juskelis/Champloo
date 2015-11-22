///scr_touching(object_a_id, object_b_id)
//checks to see if object_a is touching (directly next to) object_b
var ret = false;
with(argument0)
{
    ret = 
        place_meeting(x,y+1,argument1.id) || place_meeting(x,y-1,argument1.id)
        || place_meeting(x+1,y,argument1.id) || place_meeting(x-1,y,argument1.id);
}
return ret;