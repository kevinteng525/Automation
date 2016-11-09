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
    @dmweb.btn_id_helper.get_Dlg_btnID("Error","OK")
    @dmweb.close
  end
  
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end    
  
  
  #Test case #1: Add a User.
  def test_001_add_exist_user_webid_1651    
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($es1service, $password) 
    firstuser=@dmweb.main_administration_page.get_first_user
    @dmweb.main_administration_page.add_user_click 
    @dmweb.select_user_add_page.set_UserName(firstuser)
    @dmweb.select_user_add_page.FindUser
    @dmweb.find_user_list_page.click_FindUserList(firstuser)
    @dmweb.find_user_list_page.click_OK
    @dmweb.select_user_add_page.click_OK
   if @dmweb.main_administration_page.verifyElementExist("Error")
      puts "can't not add user which already exist, pop up a error"
      assert(true,message="can't not add user which already exist, pop up a error!")    
    else
      puts "can't not add user which already exist, not pop up a error, maybe have problem" 
      assert(false,message="can't not add user which already exist, not pop up a error, maybe have problem")      
    end    

  end   
  
  
 end

