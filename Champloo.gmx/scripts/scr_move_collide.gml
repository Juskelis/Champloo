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
    
        var hit_players = scr_player_collision(x + sign(hsp), y);
        if(array_length_1d(hit_players) > 0)
        {
            var other_player;
            for(var i = 0; i < array_length_1d(hit_players); i++)
            {
                other_player = hit_players[i];
                if(!can_dash)
                {
                    with(other_player)
                    {
                        stunned = true;
                        alarm[6] = stun_delay * room_speed;
                    }
                }
                else if(!other_player.can_dash)
                {
                    stunned = true;
                    alarm[6] = stun_delay * room_speed;
                }
                
                with(other_player)
                {
                    force_x = sign(other.hsp) * maxspeed;
                }
                force_x = -sign(hsp) * maxspeed;
            }
        }
        else
        {
            force_x = 0;
        }
        hsp = 0;
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
        
        var hit_players = scr_player_collision(x, y + sign(vsp));
        if(array_length_1d(hit_players) > 0)
        {
            var other_player;
            for(var i = 0; i < array_length_1d(hit_players); i++)
            {
                other_player = hit_players[i];
                var died = false;
                if(y < other_player.y)
                {
                    with(other_player)
                    {
                        respawning = true;
                        alarm[0] = death_time * room_speed;
                        spurt_direction = 0;
                        scr_spawn_blood(random_range(20,30), 0, 180);
                    }
                    with(obj_Score)
                    {
                        scores[other.player_number]++;
                    }
                }
                else
                {
                    died = true;
                    
                    with(other_player)
                    {
                        force_y = -jumpspeed;
                    }
                    respawning = true;
                    alarm[0] = death_time * room_speed;
                    spurt_direction = 0;
                    scr_spawn_blood(random_range(20,30), 0, 180);
                    with(obj_Score)
                    {
                        scores[other_player.player_number]++;
                    }
                }
                
                if(!died)
                {
                    force_y = -jumpspeed;
                }
            }
        }
        else
        {
            force_y = 0;
        }
        vsp = 0;
        break;
    }
}
