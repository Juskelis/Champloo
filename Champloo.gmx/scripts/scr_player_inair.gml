scr_get_input();

move = key_left + key_right;
vsp = min(vsp + grav, maxgrav);

var acceleration = 1*(30/room_speed);
hsp = clamp(hsp + move*acceleration, -maxspeed, maxspeed);

if(hsp != 0)
{
    var air_friction = 0;
    if(move == 0 || sign(move) == sign(hsp))
        air_friction = 0.4*(30/room_speed);
    else
        air_friction = 0.6*(30/room_speed);
    
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