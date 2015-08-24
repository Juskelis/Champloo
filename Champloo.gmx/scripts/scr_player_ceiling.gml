scr_get_input();
//basically operates exactly the same as normal
//  but a delay in gravity
move = key_left + key_right;

vsp = min(vsp + grav, maxgrav);
if(prev_vsp < 0) vsp = min(prev_vsp + ceiling_stick, maxgrav);

if(hsp != 0)
{
    var ceiling_friction = ceiling_stopping_friction;
    
    hsp = scr_apply_friction(hsp, ceiling_friction);
}

if(key_jump)
{
    if(abs(horizontal_amount) + abs(vertical_amount) > jumping_deadzone)
    {
        var dir = aim_direction;
        if(dir > 180)
        {
            hsp = lengthdir_x(jumpspeed, dir);
        
            dir -= 360;
            vsp = lengthdir_y(jumpspeed, (dir - 90)/4);
        }
    }
    else
        vsp = jumpspeed;
}


scr_move_collide();

///check state
if(place_meeting(x, y + 1, obj_Wall))
{
    state = States.Normal;
}
else if(!place_meeting(x, y - 1, obj_Wall))
{
    state = States.InAir;
}

if(place_meeting(x-1,y,obj_Wall) || place_meeting(x+1,y,obj_Wall))
{
    state = States.WallRiding;
}
