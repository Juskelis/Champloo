scr_get_input();

move = key_left + key_right;
vsp = clamp(
    vsp + grav*global.timescale,
    -maxgrav*global.timescale,
    maxgrav*global.timescale
);

hsp = clamp(
    hsp + move*air_acceleration*global.timescale,
    -maxspeed*global.timescale,
    maxspeed*global.timescale
);

if(hsp != 0)
{
    var air_friction = air_moving_friction;
    if(move != 0 && sign(move) != sign(hsp))
        air_friction = air_turning_friction;
    
    hsp = scr_apply_friction(hsp, air_friction*global.timescale)
}

if(key_jump_released && jumping)
{
    jumping = false;
    if(vsp < 0) vsp *= 0.5;
}

scr_move_collide();

//check state
if(!can_dash)
{
    state = States.Dashing;
}
else if(place_meeting(x, y + 1, obj_Wall))
{
    state = States.Normal;
}
else if(place_meeting(x - 1,y,obj_Wall) || place_meeting(x + 1,y,obj_Wall))
{
    state = States.WallRiding;
}
