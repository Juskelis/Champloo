if(place_meeting(x+sign(hsp),y, obj_Wall) || place_meeting(x + hsp, y, obj_Wall))
{
    while(!place_meeting(x + sign(hsp), y, obj_Wall))
    {
        x+=sign(hsp);
    }
    hsp = 0;
    force_x = 0;
}
x += hsp*global.timescale;

prev_vsp = vsp;
if(place_meeting(x,y+sign(vsp), obj_Wall) || place_meeting(x, y + vsp, obj_Wall))
{
    while(!place_meeting(x, y + sign(vsp), obj_Wall))
    {
        y += sign(vsp);
    }
    vsp = 0;
    force_y = 0;
}
y += vsp*global.timescale;
