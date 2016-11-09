require 'watir-webdriver'
require 'watir-webdriver-performance'
require_relative '..\..\Variables\SUP_Variables_Configure.rb'
require_relative '..\..\Objects\Supervisor_Objects.rb'
require_relative '..\..\Objects\SUP_Login_Page.rb'
require_relative '..\..\Objects\SUP_Header_page.rb'
require_relative '..\..\Objects\SUP_Filter_Messages_Page.rb'
require_relative '..\..\Common\Data_Read.rb'

gem "minitest"
require 'minitest/autorun'
#require_relative '..\Objects\minitest_saber_base.rb'

#Smoke test cases.
class TestSuite < Minitest::Test  
 
 def setup
   @excel_file = ReadExcel.new($excel_dir,1)
   @data_row = @excel_file.getRow   
   @supervisorweb = SUPSite.new(Watir::Browser.new)
 end
 
 def teardown
   @excel_file.closeExcel 
   @supervisorweb.close   
 end
 
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end       
    
  #All the login_logout test cases:  
  def test_sup_login_with_domain_username_webid_4753
    login_testcase(1)
  end 

  def test_sup_relogin_as_another_user_after_logout_webid_4763
    login_testcase(2)
  end 

  def test_sup_login_input_incorrect_username_webid_4751
    login_testcase(3)
  end 

  def test_sup_login_input_incorrect_password_webid_4752
    login_testcase(4)
  end 

  def test_sup_logout_webid_5485
    login_testcase(5)
  end 
        
#============================================================================================================================
  def login_testcase(i_row)
    #login supervisor web.
    @input_data = @excel_file.getUserData(@data_row,i_row)
    @supervisorweb.SUPlogin_page.login_function(@input_data)
    #verify whether login should be success or fail.
    verfiy_login   
  end

  #the function of verify whether login should be success or fail or logout.
  def verfiy_login
    if @input_data[3] == "yes"
      @supervisorweb.SUPfilter_message_page.close_filter_dialog
      @supervisorweb.load_time
      if @supervisorweb.SUPheader_page.verify_login_name($_UserDomain+'\\'+@input_data[1]) ==true
        assert(true, message=@input_data[4])  
      else
        assert(false, message=@input_data[5]) 
      end  
    elsif @input_data[3] == "no"
      #Verify whether the "Login failed. The username or password is incorrect." is display.
      if @supervisorweb.SUPlogin_page.verifyError($loginerror1) ==true
        assert(true, message=@input_data[4])  
      else
        assert(true, message=@input_data[5]) 
      end    
    else
      #logout the page.
      @supervisorweb.SUPfilter_message_page.close_filter_dialog
      @supervisorweb.load_time
      @supervisorweb.SUPheader_page.clickArrow
      @supervisorweb.SUPheader_page.clickLogout
      @supervisorweb.load_time
      if @supervisorweb.SUPlogin_page.getLoginLabel ==true
        assert(true, message=@input_data[4])    
      else
        assert(false, message=@input_data[5]) 
      end       
    end
  end  
end
 

