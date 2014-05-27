<?php
	$titre = "Menu";
	
	require_once("action/RestaurantMenuAction.php");

	$action = new RestaurantMenuAction();
	$action->execute();

	require_once('partial/site_header.php');
?>
	<div class="col-sm-2"></div>
	<div class="col-sm-8">
		<h2>Gérer un menu</h2>

		<div class="message"></div>

		<div class="row form_row">
			<div class="col-sm-4">Restaurant : </div>
			<div class="col-sm-8">
				<select id="listRestaurant" class="form-control"> 
				</select>
			</div>
		</div>

		<div class="row form_row">
			<div class="col-sm-4">Nom du menu</div>
			<div class="col-sm-8">
				<input type="text" class="form-control" id="menuName" placeholder="Nom du menu" required/>
			</div>
		</div>


		<div class="row form_row">
			<div class="table-responsive">
				<table class="table table-stripped table-hover">
					<thead>
						<tr>
							<th>Nom du plat</th>
							<th>Prix</th>
							<th>Description</th>
						</tr>
					</thead>
					<tbody id="dishList">

					</tbody>
				</table>
			</div>
		</div>

		<div class="row form_row">
			<div class="col-sm-4">Nom du plat</div>
			<div class="col-sm-8">
				<input type="text" class="form-control" id="name" placeholder="Nom du plat" required/>
			</div>
		</div>

		<div class="row form_row">
			<div class="col-sm-4">Prix</div>
			<div class="col-sm-8">
				<input type="text" class="form-control" id="price" placeholder="Prix"/>
			</div>
		</div>

		<div class="row form_row">
			<div class="col-sm-4">Description</div>
			<div class="col-sm-8">
				<textarea class="form-control" rows="3"></textarea>
			</div>
		</div>

	</div>
	<div class="col-sm-2"></div>



<?php
	require_once('partial/site_footer.php');
?>

<script>
	fillRestaurantList();
	addDish();
	addDish();
	addDish();
	addDish();
</script>