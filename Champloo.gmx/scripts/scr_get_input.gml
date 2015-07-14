//key_right = keyboard_check(right_input);
//key_left = -keyboard_check(left_input);
//key_jump = keyboard_check_pressed(jump_input);
//key_jump_held = keyboard_check(jump_input);

key_right = max(0, gamepad_axis_value(player_number, gp_axislh));
if(key_right < 0.5) key_right = 0;
key_left = min(gamepad_axis_value(player_number, gp_axislh), 0);
if(abs(key_left) < 0.5) key_left = 0;
key_jump = gamepad_button_check_pressed(player_number, gp_face1);
//key_jump_held = gamepad_button_check(0, gp_face1);
