scr_apply_forces();

repeat(abs(hsp))
{
    if(!place_meeting(x + sign(hsp), y, obj_Wall)
        && !place_meeting(x + sign(hsp), y, obj_Player))
    {
        x += sign(hsp);
    }
    else
    {
        if(place_meeting(x + sign(hsp), y, obj_Player))
        {
            var other_player = instance_place(x + sign(hsp), y, obj_Player);
            if(!other_player.respawning)
                force_x = -sign(hsp) * maxspeed;
            with(other_player)
            {
                force_x = -other.force_x;
            }
        }
        else
        {
            hsp = 0;
            force_x = 0;
        }
        break;
    }
}

repeat(abs(vsp))
{
    if(!place_meeting(x, y + sign(vsp), obj_Wall)
        && !place_meeting(x, y + sign(vsp), obj_Player))
    {
        y += sign(vsp);
    }
    else
    {
        if(place_meeting(x, y + sign(vsp), obj_Player))
        {
            var other_player = instance_place(x, y + sign(vsp), obj_Player);
            if(!other_player.respawning)
            {
                if(y < other_player.y)
                {
                    //win
                    force_y = -jumpspeed;
                    with(other_player)
                    {
                        respawning = true;
                        alarm[0] = death_time * room_speed;
                        spurt_direction = (point_direction(x,y,other.x,other.y)*3
                            + point_direction(other.x, other.y, x,y))/4;
                        
                        if(state == States.Normal)
                            spurt_direction = spurt_direction%180;
                        scr_spawn_blood(random_range(20,30), random(360), 180);
                    }
                    
                    with(obj_Score)
                    {
                        scores[other.player_number]++;
                    }
                }
                else
                {
                    //lose
                    with(other_player)
                    {
                        force_y = -jumpspeed;
                    }
                    
                    respawning = true;
                    alarm[0] = death_time * room_speed;
                    spurt_direction = (point_direction(x,y,other_player.x,other_player.y)*3
                        + point_direction(other_player.x, other_player.y, x,y))/4;
                    
                    if(state == States.Normal)
                        spurt_direction = spurt_direction%180;
                    scr_spawn_blood(random_range(20,30), random(360), 180);
                    
                    with(obj_Score)
                    {
                        scores[other_player.player_number]++;
                    }
                }
            }
        }
        else
        {
            vsp = 0;
            force_y = 0;
        }
        break;
    }
}
