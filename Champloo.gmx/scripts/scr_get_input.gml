horizontal_amount = gamepad_axis_value(player_number, gp_axislh);
vertical_amount = gamepad_axis_value(player_number, gp_axislv);
aim_direction = point_direction(x,y, x+horizontal_amount*20,y+vertical_amount*20);

key_right = max(0, horizontal_amount);
if(key_right < movement_deadzone) key_right = 0;
key_left = min(horizontal_amount, 0);
if(abs(key_left) < movement_deadzone) key_left = 0;

key_jump = gamepad_button_check_pressed(player_number, gp_face1);
key_jump_released = gamepad_button_check_released(player_number, gp_face1);

key_attack_normal = gamepad_button_check_pressed(player_number, gp_face3) && !key_block;

key_shoot_press = gamepad_button_check_pressed(player_number, gp_shoulderrb) && !key_block;
key_shoot_release = gamepad_button_check_released(player_number, gp_shoulderrb) && !key_block;

key_block = gamepad_button_check(player_number, gp_face2) && !attacking;

if(key_jump)
{
    jump_input_time = jump_input_delay;
}
else
{
    jump_input_time = max(0, jump_input_time - 1);
}

key_jump = key_jump || jump_input_time > 0;