scr_get_input();

move = key_left + key_right;
vsp = min(vsp + grav, maxgrav);

hsp = clamp(hsp + move*air_acceleration, -maxspeed, maxspeed);

if(hsp != 0)
{
    var air_friction = air_moving_friction;
    if(move != 0 && sign(move) != sign(hsp))
        air_friction = air_turning_friction;
    
    hsp = scr_apply_friction(hsp, air_friction)
}

if(key_jump_released)
{
    if(vsp < 0) vsp = 0;
}

scr_move_collide();

//check state
if(place_meeting(x, y + 1, obj_Wall))
{
    state = States.Normal;
}

if(place_meeting(x - 1,y,obj_Wall) || place_meeting(x + 1,y,obj_Wall))
{
    state = States.WallRiding;
}
