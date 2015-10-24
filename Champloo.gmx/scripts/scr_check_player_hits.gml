///scr_check_player_hits(start_x, start_y, end_x, end_y)

var cur_x = argument0;
var cur_y = argument1;
var end_x = argument2;
var end_y = argument3;

var other_player;
while(abs(cur_x - end_x) > 0 && abs(cur_y - end_y) > 0)
{
    for(var i = 0; i < instance_number(obj_Player); i++)
    {
        other_player = instance_find(obj_Player, i);
        if(other_player != id && place_meeting(cur_x,cur_y, other_player))
        {
            //do the actual checks
            
        }
    }
    cur_x = scr_approach(cur_x, end_x, 1);
    cur_y = scr_approach(cur_y, end_y, 1);
}
