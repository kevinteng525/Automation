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
    @dmweb.matter_tree_page.clickMatterClosed
    @dmweb.main_matter_page.delete_matter_click
    #@dmweb.new_search_page.click_OK
    @dmweb.Delete_Matter_page.click_delete_Matter_Ok
    @dmweb.usermenu_page.clickAdministration
    @dmweb.main_administration_page.click_seconduser
    @dmweb.main_administration_page.click_removeuser
    @dmweb.btn_id_helper.get_Dlg_btnID("Confirmation","Yes")
    @dmweb.close
  end
  
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end    
  
  
  #Test case #1: do some opearation in the user manager of matter properties
  def test_001_add_exist_user_webid_210  
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    newusername='User2'
    @dmweb.usermenu_page.clickAdministration 
    @dmweb.main_administration_page.add_user_click 
    @dmweb.select_user_add_page.set_UserName(newusername)
    @dmweb.select_user_add_page.FindUser
    @dmweb.find_user_list_page.click_FindUserList(newusername)
    @dmweb.find_user_list_page.click_OK
    @dmweb.select_user_add_page.click_OK
    @dmweb.main_administration_page.checkall_click
    @dmweb.main_head_page.clickMatterLink
    @dmweb.main_matter_page.new_matter_click
    @dmweb.new_matter_page.set_MatterName($mattername)
    @dmweb.new_matter_page.click_FindUserList(newusername)
    @dmweb.btn_id_helper.get_Dlg_btnID("Users","OK")
    @dmweb.new_matter_page.click_OK
    @dmweb.main_head_page.clickMatterLink
    @dmweb.matter_tree_page.clickMatterClosed 
    @dmweb.main_matter_page.matter_properties_click
    sleep(2)
    @dmweb.btn_id_helper.get_Dlg_btnID("test_gff - Matter Properties","User Management")
    @dmweb.new_matter_page.click_seconduser1
    @dmweb.btn_id_helper.get_Dlg_btnID("test_gff - Matter Properties","Remove")
    @dmweb.btn_id_helper.get_Dlg_btnID("Confirmation","Yes")
    @dmweb.btn_id_helper.get_Dlg_btnID("test_gff - Matter Properties","OK")
    puts "delete the add user in user manager of matter properties success"
    @dmweb.main_head_page.clickMatterLink
    @dmweb.matter_tree_page.clickMatterClosed
    @dmweb.main_matter_page.matter_properties_click
    sleep(2)
    @dmweb.btn_id_helper.get_Dlg_btnID("test_gff - Matter Properties","User Management")
    @dmweb.new_matter_page.click_firstuser1
    @dmweb.btn_id_helper.get_Dlg_btnID("test_gff - Matter Properties","Remove")
    @dmweb.btn_id_helper.get_Dlg_btnID("Confirmation","Yes")
    @dmweb.btn_id_helper.get_Dlg_btnID("Error","OK")
    @dmweb.btn_id_helper.get_Dlg_btnID("test_gff - Matter Properties","OK")
    if @dmweb.main_matter_page.staff_grid($username)
      puts "the operation for usermanager success"
      assert(true,message="the operation for usermanager success")    
    else
      puts "the operation for usermanager fail" 
      assert(false,message="the operation for usermanager fail") 
    end
    

  end   
  
  
 end

