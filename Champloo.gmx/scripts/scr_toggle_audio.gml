///scr_toggle_audio([])
if(active)
{
    global.audio_level = 1;
}
else
{
    global.audio_level = 0;
}
audio_master_gain(global.audio_level);
