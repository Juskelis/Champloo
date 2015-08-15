scr_get_input();

move = key_left + key_right;
vsp = min(vsp + grav, maxgrav);

var acceleration = 1.25*(30/room_speed);

//holding jump key stops movement input and slows
//if(jumpstate == JumpStates.Held) move = 0;

hsp = clamp(hsp + move*acceleration, -maxspeed, maxspeed);

if(hsp != 0)
{
    var ground_friction = 0;
    if(move == 0)
    {
        if(!key_jump_held)
            ground_friction = acceleration;
        else
            ground_friction = 0.5*acceleration;
    }
    else if(sign(move) != sign(hsp))
        ground_friction = 2*acceleration;
        
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

/*
hsp = 0;
vsp = 0;
*/

if(key_jump)
{
    //vsp = key_jump * -jumpspeed;
    var deadzone = 0.25;
    //move the player in the direction of stick
    if(jumpstate == JumpStates.Held && abs(horizontal_amount) + abs(vertical_amount) > deadzone)
    {
        var dir = point_direction(x,y,x + horizontal_amount, y + vertical_amount);
        hsp = lengthdir_x(jumpspeed, dir);
        
        if(dir > 270) dir -= 360;
        vsp = lengthdir_y(jumpspeed, (dir+90)/2);
    }
    else
    {
        vsp = key_jump * -jumpspeed;
    }
}

if(key_jump_high)
{
    vsp = key_jump_high*-jumpspeed;
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

scr_jumpstate_next();
