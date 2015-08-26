scr_get_input();

move = key_left + key_right;
vsp = min(vsp + grav, maxgrav);

hsp = clamp(hsp + move*acceleration, -ground_maxspeed, ground_maxspeed);

if(hsp != 0)
{
    var ground_friction = 0;
    if(move == 0)
        ground_friction = stopping_friction;
    else
    {
        if(sign(move) != sign(hsp))
            ground_friction = turning_friction;
        else
            ground_friction = moving_friction;
    }
    
    hsp = scr_apply_friction(hsp, ground_friction);
}

if(key_jump)
{
    if(false && abs(horizontal_amount) + abs(vertical_amount) > jumping_deadzone)
    {
        var dir = aim_direction;
        hsp = lengthdir_x(jumpspeed, dir);
        
        if(dir > 270) dir -= 360; //mapping from -180 to 180
        vsp = lengthdir_y(jumpspeed, (dir+90)/2); //average between normal and dir
    }
    else
        vsp = -jumpspeed;
        
    jump_input_time = 0;
}

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
