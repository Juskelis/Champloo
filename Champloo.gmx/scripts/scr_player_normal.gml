scr_get_input();

move = key_left + key_right;
vsp = min(vsp + grav*global.timescale, maxgrav*global.timescale);

hsp = clamp(
    hsp + move*acceleration*global.timescale,
    -ground_maxspeed*global.timescale,
    ground_maxspeed*global.timescale
);

if(hsp != 0)
{
    var ground_friction = 0;
    if(block_slow)
    {
        ground_friction = blocking_friction;
    }
    else if(move == 0)
        ground_friction = stopping_friction;
    else
    {
        if(sign(move) != sign(hsp))
            ground_friction = turning_friction;
        else
            ground_friction = moving_friction;
    }
    
    hsp = scr_apply_friction(hsp, ground_friction*global.timescale);
}

if(key_jump)
{
    vsp = -jumpspeed;
        
    jump_input_time = 0;
}

//apply forces
vsp = min(vsp + force_y*global.timescale, maxgrav*global.timescale);
hsp = clamp(
    hsp + force_x*global.timescale,
    -ground_maxspeed*global.timescale,
    ground_maxspeed*global.timescale
);


scr_move_collide();

///check state
if(!place_meeting(x, y + 1, obj_Wall))
{
    state = States.InAir;
}

if(place_meeting(x - 1,y,obj_Wall) || place_meeting(x + 1,y,obj_Wall))
{
    state = States.WallRiding;
}
