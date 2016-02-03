///scr_place_meeting_but(x, y, obj_type, instance)

//arguments
var _x = argument0;
var _y = argument1;
var _obj_type = argument2;
var _instance = argument3;

var occupied = false;
var curr_wall = noone;
for(var i = 0; i < instance_number(_obj_type) && !occupied; i++)
{
    curr_wall = instance_find(_obj_type, i);
    if(curr_wall.id != _instance.id
        && place_meeting(_x, _y, curr_wall))
    {
        occupied = true;
    }
}
return occupied;
