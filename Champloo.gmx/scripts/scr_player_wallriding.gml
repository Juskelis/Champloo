scr_get_input();

move = key_left + key_right;
vsp = min(vsp + grav, maxgrav/2);

//if(key_jump_held) move = 0;

var wall_left = place_meeting(x - 1, y, obj_Wall);
var wall_right = place_meeting(x + 1, y, obj_Wall);
var walldir = wall_right - wall_left;

if(move == 0 || sign(move) == sign(walldir))
{
    movespeed = walldir;
}
else
{
    movespeed = movespeed + (sign(move)*(1/(0.2*room_speed)));
}
hsp = movespeed;

/*
hsp = 0;
vsp = 0;
*/

if(key_jump)
{
    /*
    if(sign(move) == sign(walldir))
    {
        hsp = (-walldir)*maxspeed*0.5;
        vsp = -jumpspeed;
    }
    else
    {
        hsp = (-walldir)*maxspeed;
        vsp = -jumpspeed*0.8;
    }
    */
    
    if(sign(move) == sign(walldir))
    {
        hsp = (-walldir)*maxspeed*0.5;
        vsp = -jumpspeed;
    }
    else
    {
        hsp = (-walldir)*maxspeed;
        vsp = -jumpspeed*0.8;
    }
    
    //move the player in the direction of stick
    //if(jumpstate == JumpStates.Held && vertical_amount > 0)
    if(vertical_amount > 0.1)
    {
        var dir = point_direction(x,y,x + horizontal_amount, y + vertical_amount);
        hsp = lengthdir_x(jumpspeed, dir);
        vsp = lengthdir_y(jumpspeed, dir);
    }
}

if(key_jump_high)
{
        hsp = (-walldir)*maxspeed;
        vsp = -jumpspeed*0.8;
}

scr_move_collide();

//get next state
if(!place_meeting(x + 1, y, obj_Wall) && !place_meeting(x - 1, y, obj_Wall))
{
    if(!place_meeting(x, y + 1, obj_Wall))
    {
        movespeed = 0;
        state = States.InAir;
    }
    else
    {
        movespeed = 0;
        state = States.Normal;
    }
}

scr_jumpstate_next();
