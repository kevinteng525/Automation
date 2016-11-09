require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
require_relative '..\Objects\Login_page.rb'
require_relative '..\Objects\Main_Matter_page.rb'
require_relative '..\Objects\New_Matter_page.rb'
require_relative '..\Objects\Matter_Tree_page.rb'
require_relative '..\Objects\New_Search_page.rb'
require_relative '..\Objects\DM_Main_Head_page.rb'
require_relative '..\Objects\CollectionArea_page.rb'
require_relative '..\Objects\Search_Result_page.rb'
require_relative '..\Objects\Export_Setting_page.rb'
require_relative '..\Objects\Export_Summary_page.rb'
require_relative '..\Objects\Main_Identity_page.rb'
require_relative '..\Objects\New_Identity_page.rb'
require_relative '..\Objects\Delete_Identity_page.rb'
require_relative '..\Objects\Delete_Matter_page.rb'
require_relative '..\Objects\Main_Administration_page.rb'
require_relative '..\Objects\Find_User_List_page.rb'
require_relative '..\Objects\Select_User_Add_page.rb'
require_relative '..\Objects\Delete_User_page.rb'
require_relative '..\Objects\All_Items_page.rb'
require_relative '..\Objects\Matter_tree_items.rb'
gem "minitest"
require 'minitest/autorun'
require_relative '..\Objects\minitest_saber_base.rb'
require_relative '..\Objects\saber_base.rb'
#Smoke test cases.
class TestSuite < SaberTestBase
  include SaberBase
  
  def setup
    
    @dmweb = Site.new(Watir::Browser.new)
  end
  
  def teardown
       @dmweb.usermenu_page.clickAdministration
       firstuserstatus=@dmweb.main_administration_page.get_firstuser_personalmatter_status
       if firstuserstatus.include?"Yes"
       @dmweb.main_administration_page.click_firstuser_personalmatter
       @dmweb.main_administration_page.remove_personalmatter_click
       @dmweb.btn_id_helper.get_Dlg_btnID("Confirmation","Yes")
       end
       @dmweb.close
  end
  
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end    
  
  
  #Test case #1: Add a User.
  def test_add_personalmatter_webid_176    
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    @dmweb.usermenu_page.clickAdministration
    firstuser=@dmweb.main_administration_page.get_first_user
    firstuserstatus=@dmweb.main_administration_page.get_firstuser_personalmatter_status
    if firstuserstatus.include? "No"
       @dmweb.main_administration_page.click_firstuser_personalmatter
       @dmweb.main_administration_page.add_personalmatter_click
       firstusernewstatus=@dmweb.main_administration_page.get_firstuser_personalmatter_status
       if firstusernewstatus.include? "Yes"
       puts "#{firstuser} add personal matter success"
       else 
       puts "#{firstuser} add personal matter failed"
       end
       @dmweb.main_head_page.clickMatterLink
       sleep(5)
       personalmattername="Personal Matter - "+$deleteUserNameO
       if @dmweb.Mattertree_items.matteritems_mymatters(personalmattername,0).exist?
       puts "add the personal matter have work correct"
       assert(true,message="add the personal matter have work correct")    
       else
       puts "add the personal matter have not work correct" 
       assert(false,message="add the personal matter have not work correct")    
       end    
    
   end   
  end   
  
  
 end

