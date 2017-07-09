#! /usr/bin/env lua
-- 

function buildBuildingPlacementPanel(UI, world)
	panel = UI["creative_panel"]
	for key, prototype in pairs(world.registry.buildingsRegistry)
	do
		print(prototype.id)
		button = Button.Create(prototype.id)
		panel.Add(button)
		button.Text = prototype.id
		function printID()
			print(prototype.id)
			-- world.
			world.SetMode(WorldMode.Build, { prototype.id });
		end
		button.OnClick(printID)
	end
end

function SelectionViewOnUpdate(UI, world)
	if world.selectedTile != nil then
		UI["tile_coord"].Text = world.selectedTile.ToString()
	end
end