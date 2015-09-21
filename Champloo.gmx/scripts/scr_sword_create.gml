//argument0: player object
//argument1: attack_style
//argument2: attack_speed
//argument3: attack_direction

var sword = instance_create(argument0.x, argument0.y, obj_Sword);
with(sword)
{
    attack_style = argument1;
    attack_speed = argument2;
    attack_direction = argument3;
    
    if(attack_direction < 90 || attack_direction > 270)
    {
        start_direction = attack_direction + 45;
        end_direction = start_direction - 90;
    }
    else
    {
        start_direction = attack_direction - 45;
        end_direction = start_direction + 90;
    }
    current_direction = start_direction;
    
    /*
    current_direction = attack_direction/2 + start_direction;
    start_direction = current_direction;
    if(attack_direction < 90)
        end_direction = start_direction - 90;
    else
        end_direction = start_direction + 90;
    */
    direction_step = (end_direction - start_direction)/(attack_speed*room_speed);
    alarm[0] = attack_speed*room_speed;
    
    show_debug_message(start_direction);
    
    image_speed = image_speed/attack_speed;
    image_angle = current_direction;
}
return sword;