//argument0: player object
//argument1: attack_style
//argument2: attack_speed
//argument3: attack_direction

var sword = instance_create(argument0.x, argument0.y, obj_Sword);
with(sword)
{
    player = argument0;
    attack_style = argument1;
    attack_speed = argument2;
    attack_direction = argument3;
    
    current_direction = attack_direction + start_direction;
    direction_step = (end_direction - start_direction)/(attack_speed*room_speed);
    alarm[0] = attack_speed*room_speed;
    
    image_speed = image_speed/attack_speed;
}
