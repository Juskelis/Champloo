/*
    flags/states:
        swinging (attacking)
        recharging attack
        blocking
        coming out of block (can block again, but cannot attack)
        
*/

if(attacking || throwing || !has_sword)// || true)
{
    key_block = false;
    key_attack_normal = false;
    key_shoot_press = false;
    key_shoot_release = false;
}
//key_shoot_press = false;


if(key_right < movement_deadzone) key_right = 0;
if(abs(key_left) < movement_deadzone) key_left = 0;


if(key_jump)
{
    jump_input_time = jump_input_delay;
}
else
{
    jump_input_time = max(0, jump_input_time - 1);
}

key_jump = key_jump || jump_input_time > 0;

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
