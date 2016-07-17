///scr_change_volume([isIncrease, amount])
if(argument0[0])
    global.audio_level += argument0[1];
else
    global.audio_level -= argument0[1];

global.audio_level = clamp(global.audio_level, 0, 1);
value = global.audio_level;
