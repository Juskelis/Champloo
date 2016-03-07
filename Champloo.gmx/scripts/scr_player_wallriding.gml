scr_get_input();

move = key_left + key_right;
vsp = clamp(
    vsp + grav*global.timescale,
    -(maxgrav*global.timescale)/2,
    (maxgrav*global.timescale)/2
);

var wall_left = place_meeting(x - 1, y, obj_Wall);
var wall_right = place_meeting(x + 1, y, obj_Wall);
var walldir = wall_right - wall_left;

if(move == 0 || sign(move) == sign(walldir))
{
    movespeed = walldir;
    hsp = movespeed;
}
else
{
    movespeed = movespeed + (sign(move)*(1/(walk_off_wall_time*room_speed)));
    hsp = movespeed;
}


if(place_meeting(x,y+1, obj_Wall) || place_meeting(x, y - 1, obj_Wall))
{
    movespeed = move;
}


if(key_jump)
{
    if(sign(move) == sign(walldir))
    {
        hsp = (-walldir)*maxspeed*0.5;
        vsp = -jumpspeed;
        audio_play_sound(snd_Jump_Small, 0, false);
    }
    else
    {
        hsp = (-walldir)*maxspeed;
        vsp = -jumpspeed*0.8;
    
        //move the player in the direction of stick
        if(vertical_amount > jumping_deadzone/2)
        {
            hsp = lengthdir_x(jumpspeed, aim_direction);
            vsp = lengthdir_y(jumpspeed, aim_direction);
        }
        audio_play_sound(snd_Jump, 0, false);
    }
    jump_input_time = 0;
}

if(abs(dash_force_x) == 0 && abs(dash_force_y) == 0)
{
    available_dashes = max(available_dashes, 1);
}

scr_move_collide();

//get next state
if(!place_meeting(x + 1, y, obj_Wall) && !place_meeting(x - 1, y, obj_Wall))
{
    if(place_meeting(x, y + 1, obj_Wall))
    {
        movespeed = 0;
        state = States.Normal;
    }
    else
    {
        movespeed = 0;
        state = States.InAir;
    }
}
