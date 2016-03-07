///scr_outside_room( padding = 0, wrap = global.screen_wrapping)

var padding = 0;
if(argument_count > 0) padding = argument[0];

var wrap = global.screen_wrapping;
if(argument_count > 1) wrap = argument[1];

if(wrap)
{
    if(bbox_top > room_height + padding) y = -sprite_yoffset;
    else if(bbox_bottom < -padding) y = room_height + sprite_yoffset;
    
    if(bbox_left > room_width + padding) x = -sprite_xoffset;
    else if(bbox_right < -padding) x = room_width + sprite_xoffset;
}
else
{
    var outside = false;
    
    if(bbox_top > room_height + padding) outside = true;
    else if(bbox_bottom < -padding) outside = true;
    
    if(bbox_left > room_width + padding) outside = true;
    else if(bbox_right < -padding) outside = true;
    
    if(outside)
    {
        if(is(obj_Player)){
            with(obj_Score)
            {
                scores[other.player_number] = max(0, scores[other.player_number]-1);
            }
        }
        instance_destroy();
    }
}
