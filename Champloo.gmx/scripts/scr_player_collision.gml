///scr_player_collision(x,y)
//  returns array of all colliding players
//  or noone if none are there

var temp_x = argument0;
var temp_y = argument1;

var num_players = 0;
var other_player;
var return_list;
for(var i = 0; i < instance_number(obj_Player); i++)
{
    other_player = instance_find(obj_Player, i);
    if(other_player.id != id && !other_player.respawning
        && place_meeting(temp_x, temp_y, other_player))
    {
        return_list[num_players] = other_player;
        num_players++;
    }
}

if(num_players > 0)
    return return_list;
return noone;