///scr_block_check(damage)

if(attacking)
    return false;

if(key_block)
{
    block_level = max(0, block_level - argument0);
    return block_level > 0;
}

return false;
