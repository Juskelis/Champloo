scr_get_input();

move = key_left + key_right;
vsp = min(vsp + grav, maxgrav);

var acceleration = 1.5*(30/room_speed);
hsp = clamp(hsp + move*acceleration, -maxspeed, maxspeed);

if(hsp != 0)
{
    var ground_friction = 0;
    if(move == 0)
        ground_friction = 1.5*(30/room_speed);
    else if(sign(move) != sign(hsp))
        ground_friction = 2.5*(30/room_speed);
        
    var hsptest = hsp - sign(hsp)*ground_friction;
    if(sign(hsptest) != sign(hsp))
    {
        hsp = 0;
    }
    else
    {
        hsp = hsptest;
    }
}

if(key_jump)
{
    vsp = key_jump * -jumpspeed;
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