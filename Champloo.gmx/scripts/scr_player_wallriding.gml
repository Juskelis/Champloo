scr_get_input();

move = key_left + key_right;
vsp = min(vsp + grav, maxgrav/2);

var wall_left = place_meeting(x - 1, y, obj_Wall);
var wall_right = place_meeting(x + 1, y, obj_Wall);
var walldir = wall_right - wall_left;

if((!place_meeting(x,y+1, obj_Wall) && !place_meeting(x, y - 1, obj_Wall))
    && (move == 0 || sign(move) == sign(walldir)))
{
    movespeed = walldir;
}
else
{
    movespeed = movespeed + (sign(move)*(1/(0.2*room_speed)));
}
hsp = movespeed;

if(key_jump)
{
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
    if(vertical_amount > jumping_deadzone/2)
    {
        hsp = lengthdir_x(jumpspeed, aim_direction);
        vsp = lengthdir_y(jumpspeed, aim_direction);
    }
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
    else if(place_meeting(x, y - 1, obj_Wall))
    {
        movespeed = 0;
        state = States.Ceiling;
    }
    else
    {
        movespeed = 0;
        state = States.InAir;
    }
}
