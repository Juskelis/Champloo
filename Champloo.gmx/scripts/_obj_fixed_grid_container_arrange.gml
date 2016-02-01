//items arrangement
var i, j;

var num_x_cells = ds_grid_width(items);
var num_y_cells = ds_grid_height(items);

var cell_x = 0;
var cell_y = 0;

var max_height = 0;

var curr_cell = noone;

for(i = 0; i < num_y_cells; i++)
{
    for(j = 0; j < num_x_cells; j++)
    {
        curr_cell = ds_grid_get(items, j, i);
        with(curr_cell)
        {
            component_shift_to(
                cell_x,
                cell_y
            );
            
            if(height > max_height)
                max_height = height;
                
            cell_x += width + other.horizontal_padding;
        }
    }
    cell_y += max_height + vertical_padding;
}

/*
var cell_width = width/num_x_cells;
var cell_height = height/num_y_cells;

for(i = 0; i < num_y_cells; i++) {
    for(j = 0; j < num_x_cells; j++) {
        with(ds_grid_get(items, j, i)) {
            component_shift_to(
                j * cell_width + cell_width/2,
                i * cell_height + cell_height/2
            );
        }
    }
}
*/
