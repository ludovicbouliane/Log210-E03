<?php
	session_start();

	abstract class CommonAction{
		public static $PUBLIC_ACCOUNTTYPE = 0;
		public static $CLIENT_ACCOUNTTYPE = 1;
		public static $RESTAURANTMANAGER_ACCOUNTTYPE = 2;
		public static $CONTRACTOR_ACCOUNTTYPE = 3;
		public static $ADMIN_ACCOUNTTYPE = 4;

		private $pageVisibility;

		public function __construct($pageVisibility){
			$this->pageVisibility = $pageVisibility;	
		}

		public function execute(){
			//If the user logs out, we flush all data about him
			if(isset($_GET["logout"])){
				session_unset();
				session_destroy();
				session_start();
			}
		
			if (!isset($_SESSION["AccountType"])) {
				$_SESSION["AccountType"] = CommonAction::$PUBLIC_ACCOUNTTYPE;
			}

			if(! in_array($_SESSION["AccountType"], $this->pageVisibility)){
				if($_SESSION["AccountType"] !== CommonAction::$ADMIN_ACCOUNTTYPE){
					header("location:/index");
					exit;
				}
			}	
			
			$this->executeAction();
		}

		protected abstract function executeAction();
		
		protected function isLoggedIn(){
			return $_SESSION["AccountType"] > CommonAction::$PUBLIC_ACCOUNTTYPE;
		}

		public function getUsername(){
			return $_SESSION["Username"];
		}

		public function getAccountType(){
			return $_SESSION["AccountType"];
		}
	}