if(attacking || throwing || !has_sword)// || true)
{
    key_block = false;
    key_attack_normal = false;
    key_shoot_press = false;
    key_shoot_release = false;
}


if(key_right < movement_deadzone) key_right = 0;
if(abs(key_left) < movement_deadzone) key_left = 0;


if(key_jump)
{
    jump_input_time = jump_input_delay;
}
else
{
    jump_input_time = scr_approach(jump_input_time, 0, 1);
}

key_jump = key_jump || jump_input_time > 0;

if(key_attack_normal)
{
    attack_input_time = attack_input_delay;
}
else
{
    attack_input_time = scr_approach(attack_input_time, 0, 1);
}

key_attack_normal = key_attack_normal || attack_input_time > 0;

if(key_parry)
{
    parry_input_time = parry_input_delay;
}
else
{
    parry_input_time = scr_approach(parry_input_time, 0, 1);
}

key_parry = key_parry || parry_input_time > 0;

if(key_block)
{
    key_jump = false;
    key_right = 0;
    key_left = 0;
}

if(stunned)// && false)
{
    key_jump = false;
    key_right = 0;
    key_left = 0;
}

if(!can_dash)
{
    horizontal_amount = 0;
    vertical_amount = 0;
    
    key_right = 0;
    key_left = 0;
    
    key_jump = false;
    key_jump_released = false;
    
    key_block = false;
    key_attack_normal = false;
    
    key_shoot_press = false;
    key_shoot_release = false;
    
    key_dash = false;
    
    key_parry = false;
}
