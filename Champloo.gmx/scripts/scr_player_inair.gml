scr_get_input();

move = key_left + key_right;
vsp = min(vsp + grav, maxgrav);

var acceleration = 1*(30/room_speed);
hsp = clamp(hsp + move*acceleration, -maxspeed, maxspeed);

if(hsp != 0)
{
    var air_friction = 0;
    if(move == 0 || sign(move) == sign(hsp))
        air_friction = 0.4*acceleration;
    else
        air_friction = 0.6*acceleration;
    
    var hsptest = hsp - sign(hsp)*air_friction;
    if(sign(hsptest) != sign(hsp))
    {
        hsp = 0;
    }
    else
    {
        hsp = hsptest;
    }
}

if(key_jump_released || key_jump_high_released)
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

scr_jumpstate_next();
