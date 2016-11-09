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
    @dmweb.btn_id_helper.get_Dlg_btnID("Confirmation","No")
    @dmweb.usermenu_page.clickAdministration
    userfullname=$username1+' ('+$username1+'@'+$_UserDomain+')'
    @dmweb.main_administration_page.user_select_click(userfullname.capitalize)
    @dmweb.main_administration_page.remove_user_click
    @dmweb.btn_id_helper.get_Dlg_btnID("Confirmation","Yes") 
    @dmweb.close
  end
  
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end 
  
def test_adduser_focus_webid_1666
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    @dmweb.usermenu_page.clickAdministration
    @dmweb.main_administration_page.add_user_click 
    @dmweb.select_user_add_page.set_UserName($username1)
    @dmweb.select_user_add_page.FindUser
    @dmweb.find_user_list_page.click_FindUserList($username1.capitalize)
    @dmweb.find_user_list_page.click_OK
    @dmweb.select_user_add_page.click_OK
    @dmweb.main_administration_page.checkall_click
    sleep(5)
    @dmweb.main_administration_page.remove_user_click
    a=@dmweb.btn_id_helper.dlg_Parent_Div("Confirmation")
    if @dmweb.btn_id_helper.getdlg_content(a).text.include? $username1
      puts "add user #{$username1} focus success!"
      assert(true,message="add user #{$username1} focus success!")
      
    else
      puts "add user #{$username1} focus failed!"
      assert(false,message="add user #{$username1} focus failed!")
    end
 end
 
end  