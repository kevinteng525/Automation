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
require_relative '..\Objects\edit_matterproperties.rb'
gem "minitest"
require 'minitest/autorun'
require_relative '..\Objects\minitest_saber_base.rb'
require_relative '..\Objects\saber_base.rb'
#Smoke test cases.
class TestSuite < SaberTestBase
  include SaberBase
  
  def setup
    #ingest_test_data_to_souceone
    #@dmweb = Site.new(Watir::Browser.new)
  end
  
  def teardown
    #this insure the browser will be closed when exception met during testing
    @dmweb = Site.new(Watir::Browser.new)
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    @dmweb.main_head_page.clickMatterLink
    sleep(5)
    if @dmweb.matter_tree_page.verifyMatterInTree($mattername)
    @dmweb.matter_tree_page.clickMatterClosed
    @dmweb.main_matter_page.delete_matter_click
    @dmweb.Delete_Matter_page.click_delete_Matter_Ok
    end
    
    @dmweb.close
  end
  
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end    
  
  
  #Test case #1: Add a User.
 def addUser
    @dmweb = Site.new(Watir::Browser.new) 
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($es1service, $password) 
    @dmweb.main_administration_page.add_user_click 
    @dmweb.select_user_add_page.set_UserName($username)
    @dmweb.select_user_add_page.FindUser
    @dmweb.find_user_list_page.click_FindUserList($username)
    @dmweb.find_user_list_page.click_OK
    @dmweb.select_user_add_page.click_OK
    sleep(3)
    @dmweb.main_administration_page.checkall_click
    if @dmweb.main_administration_page.verifyUserInGrid($deleteusername) || @dmweb.main_administration_page.verifyUserInGrid($deleteUserNameO)#deleteusername
      puts "test case 1: Add user success!"
      assert(true,message="test case 1: Add user success!")    
    else
      puts    "test case 1: Add user failed!" 
      assert(false,message="test case 1: Add user failed!")      
    end    
    sleep(6)
    @dmweb.close
  end   
  
  #Test case #2: verify login.
  def login
    @dmweb = Site.new(Watir::Browser.new)
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    
    if @dmweb.useraccount_Page.logged_in($username)
      puts "test case 2: Login success!"
      assert(true,message="test case 2: Login success!")      
    else  
      puts "test case 2: Login failed!" 
      assert(false,message="test case 2: Login failed!")
    end
    @dmweb.close
  end
  
  #Test case #3: New a matter. 
  def newMatter
    @dmweb = Site.new(Watir::Browser.new)
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    @dmweb.main_matter_page.new_matter_click
    @dmweb.new_matter_page.set_MatterName($mattername)
    @dmweb.new_matter_page.click_OK
   sleep(2)
    @dmweb.main_head_page.clickMatterLink
    @dmweb.matter_tree_page.clickMatterClosed
    @dmweb.main_matter_page.matter_properties_click
    sleep(2)
    @dmweb.btn_id_helper.get_Dlg_btnID("#{$mattername} - Matter Properties","Search Folders")
    @dmweb.Edit_matterproperties.allsearch_checkbox("25EmbeddedFoldersMappedFolder")
    @dmweb.btn_id_helper.get_Dlg_btnID("#{$mattername} - Matter Properties","OK")
 
    #verify the matter is created.
    if @dmweb.matter_tree_page.verifyMatterInTree($mattername)
      puts "test case 3: Create matter success!"
      assert(true,message="test case 3: Create matter success!")
    else  
      puts "test case 3:Create matter failed!"
      assert(false,message="test case 3:Create matter failed!")
    end
    @dmweb.close
  end
  
  #Test case #4: New a search.
  def newSearch
    @dmweb = Site.new(Watir::Browser.new)
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    @dmweb.matter_tree_page.expandMatterNode
    @dmweb.matter_tree_page.clickLastCollectionarea
    @dmweb.main_matter_page.new_search_click
    @dmweb.new_search_page.set_SearchName($searchname)
    @dmweb.new_search_page.searchclick_OK
 
    #verify the search is created.
    @dmweb.main_head_page.clickMatterLink
    @dmweb.matter_tree_page.expandMatterNode
    @dmweb.matter_tree_page.clickLastCollectionarea
    if @dmweb.search_summary_page.validSearchName($searchname)
      puts "test case 4: Search is created success!"
      assert(true,message="test case 4: Search is created success!")
    else  
      puts "test case 4: Search is created failed!"  
      assert(false,message="test case 4: Search is created failed!")
    end 
    @dmweb.close
   end
  
   #Test case #5: Run a search. 
   def runSearch
    @dmweb = Site.new(Watir::Browser.new)
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    
  
    
    @dmweb.main_head_page.clickMatterLink
    @dmweb.matter_tree_page.expandMatterNode
    @dmweb.matter_tree_page.expandCollectionAreaNode
    @dmweb.matter_tree_page.clickCollapsedSearchNode
    @dmweb.search_result_page.clickSearchFindButton
    sleep(30)
    
    #verify the search is run completed.
    @dmweb.main_head_page.clickMatterLink
    @dmweb.matter_tree_page.expandMatterNode
    @dmweb.matter_tree_page.clickLastCollectionarea
    if @dmweb.search_summary_page.verifySearchStatus
      puts "test case 5: Search run success!"
      assert(true,message="test case 5: Search run success!")
    else  
      puts "test case 5: Search run failed!"
      assert(false,message="test case 5: Search run failed!")
    end
    @dmweb.close
  end
 
   #Test case #6: Assign items to matter. 
   def assignToMatter
    @dmweb = Site.new(Watir::Browser.new)
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    @dmweb.matter_tree_page.expandMatterNode
    @dmweb.matter_tree_page.expandCollectionAreaNode
    @dmweb.matter_tree_page.clickCollapsedSearchNode
    @dmweb.search_result_page.checkAllResult
    @dmweb.search_result_page.clickAssign("Assign")
    @dmweb.search_result_page.enterAssignToMatter
    @dmweb.matter_tree_page.clickMatterReviewPlusNode
    @dmweb.matter_tree_page.clickAllItemsNode
    sleep(2)
    #verify all the items are assigned success.
    if @dmweb.allItems_result_page.verifyTotalInAllItems($searchnumber)
       puts "test case 6: Assign items to matter success!"
      assert(true,message="test case 6: Assign items to matter success!")
    else  
      puts "test case 6: Assign items to matter failed!"
      assert(false,message="test case 6: Assign items to matter failed!")
    end
    
    @dmweb.close
  end

  #Test case #7: Create a export
  def newExport
    @dmweb = Site.new(Watir::Browser.new)
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    @dmweb.matter_tree_page.expandMatterNode
    @dmweb.matter_tree_page.expandCollectionAreaNode
    @dmweb.matter_tree_page.clickCollapsedSearchNode
    @dmweb.search_result_page.checkAllResult
    @dmweb.search_result_page.clickExport
    @dmweb.search_result_page.enterExport
    @dmweb.export_setting_page.selectContainerType('Native Container')
    @dmweb.export_setting_page.setFileLocation($exportfilelocation)
    @dmweb.export_setting_page.clickOK
    #verify the export is OK.
    @dmweb.matter_tree_page.clickExportNodeExpanded
    if @dmweb.export_summary_page.validExportName($mattername)
      puts "test case 7: Export is created success!"
      assert(true,message="test case 7: Export is created success!")
    else  
      puts "test case 7: Export is created failed!"
      assert(false,message="test case 7: Export is created failed!")
    end
    @dmweb.close
  end
  
  #Test case #8: Delete a matter.
  def deleteMatter
    @dmweb = Site.new(Watir::Browser.new)
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    @dmweb.matter_tree_page.clickMatterClosed
    @dmweb.main_matter_page.delete_matter_click
    #@dmweb.new_search_page.click_OK
    @dmweb.Delete_Matter_page.click_delete_Matter_Ok
    #verify the matter is deleted.
    if @dmweb.matter_tree_page.verifyMatterInTree($mattername)
      puts "test case 8: Delete matter failed!"
      assert(false,message="test case 8: Delete matter failed!")
    else  
      puts "test case 8: Delete matter success!" 
      assert(true,message="test case 8: Delete matter success!")
    end
    @dmweb.close
  end
  
  #Test case #9: New a identity.
  def newIdentity
    @dmweb = Site.new(Watir::Browser.new)
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)   
    @dmweb.usermenu_page.ClickIdentities 
    @dmweb.main_identity_page.new_Identity_click
    @dmweb.new_identity_page.set_IdentityName($identityname)
    @dmweb.new_identity_page.click_Save_Close
    if @dmweb.main_identity_page.verifyIdentityInGrid($identityname)
      puts "test case 9: Create identity success!"
      assert(true,message="test case 9: Create identity success!")
    else  
      puts "test case 9: Create identity failed!"
      assert(false,message="test case 9: Create identity failed!")
    end
    @dmweb.close
  end
  
  # #Test case #10: Delete a identity.
  def deleteIdentity
    @dmweb = Site.new(Watir::Browser.new)
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password) 
    @dmweb.usermenu_page.ClickIdentities  
    @dmweb.main_identity_page.ClickIdentityInGrid($identityname)  
    @dmweb.main_identity_page.delete_Identity_click
    @dmweb.delete_identity_page.click_delete_identity_Yes
    #verify the identity is deleted.
    if @dmweb.main_identity_page.verifyIdentityInGrid($identityname)
      puts "test case 10: Delete identity failed!"
      assert(false,message="test case 10: Delete identity failed!")
    else  
      puts "test case 10: Delete identity success!"  
      assert(true,message="test case 10: Delete identity success!")
    end  
    @dmweb.close
  end
  
  
  #Test case #11: Delete the add User.
  def deleteUser
    @dmweb = Site.new(Watir::Browser.new)
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($es1service, $password) 
    @dmweb.main_administration_page.click_MainUserList
    @dmweb.main_administration_page.remove_user_click
    @dmweb.delete_user_page.click_delete_user_Yes
    if @dmweb.main_administration_page.verifyUserInGrid($deleteusername) || @dmweb.main_administration_page.verifyUserInGrid($deleteUserNameO)
      puts "test case 11: Delete user failed!"
      assert(false,message="test case 11: Delete user failed!")      
    else  
      puts "test case 11: Delete user success!" 
      assert(true,message="test case 11: Delete user success!")
    end
    @dmweb.close
  end
  
  def test_workflow_webid_1787
      
      
    login
    newMatter
    newSearch
    runSearch
    assignToMatter
    newExport
    deleteMatter
    newIdentity
    deleteIdentity
    
  end
 end
 

