//key_right = keyboard_check(right_input);
//key_left = -keyboard_check(left_input);
//key_jump = keyboard_check_pressed(jump_input);
//key_jump_held = keyboard_check(jump_input);

horizontal_amount = gamepad_axis_value(player_number, gp_axislh);
vertical_amount = gamepad_axis_value(player_number, gp_axislv);
//if(vertical_amount > 0.5) vertical_amount = 0;

key_right = max(0, horizontal_amount);
if(key_right < 0.5) key_right = 0;
key_left = min(horizontal_amount, 0);
if(abs(key_left) < 0.5) key_left = 0;

key_jump_pressed = gamepad_button_check_pressed(player_number, gp_face1);
key_jump = gamepad_button_check_released(player_number, gp_face1);
key_jump_held = gamepad_button_check(player_number, gp_face1);

key_attack_normal = gamepad_button_check_pressed(player_number, gp_face2);