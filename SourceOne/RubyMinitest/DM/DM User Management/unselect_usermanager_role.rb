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
require_relative '..\Objects\New_Matter_Dialog.rb'
require_relative '..\Objects\Matter_Properties_Dialog.rb'
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
    @dmweb1.close
    @dmweb2 = Site.new(Watir::Browser.new)
    @dmweb2.login_page.open($DMWebSite)
    @dmweb2.login_page.login_as($username, $password)
    @dmweb2.usermenu_page.clickAdministration
    userfullname=$username1+' ('+$username1+'@'+$_UserDomain+')'
    @dmweb2.main_administration_page.user_select_click(userfullname.capitalize)
    @dmweb2.main_administration_page.remove_user_click
    @dmweb2.btn_id_helper.get_Dlg_btnID("Confirmation","Yes") 
    @dmweb2.close
  end
  
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end 
  
def test_unselect_usermanager_webid_264
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    @dmweb.usermenu_page.clickAdministration
    @dmweb.main_administration_page.add_user_click 
    @dmweb.select_user_add_page.set_UserName($username1)
    @dmweb.select_user_add_page.FindUser
    @dmweb.find_user_list_page.click_FindUserList($username1.capitalize)
    @dmweb.find_user_list_page.click_OK
    @dmweb.select_user_add_page.click_OK
    [2,3,4].each do |x|
       @dmweb.search_result_page.search_each(x).wait_until_present
       @dmweb.search_result_page.search_each(x).click
    end
    @dmweb.close
    @dmweb1 = Site.new(Watir::Browser.new)
    @dmweb1.login_page.open($DMWebSite)
    @dmweb1.login_page.login_as($username1, $password)
    if !@dmweb1.usermenu_page.administration_menu.exist?
      puts "the user #{$username1} can't have administration page success!"
      assert(true,message="the user #{$username1} can't have administration page success!")
      
    else
      puts "the user #{$username1} can't have administration page failed!"
      assert(false,message="the user #{$username1} can't have administration page failed!")
    end
 end
 
end  