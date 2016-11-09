require 'watir-webdriver'
require 'watir-webdriver-performance'
require_relative '..\Variables\SUP_Variables_Configure.rb'
require_relative '..\Objects\Supervisor_Objects.rb'
require_relative '..\Common\Data_Read.rb'
gem "minitest"
require "minitest/autorun"

#Login page==============================================================
class SUPLoginPage < BrowserContainer

  #login function.
  def login_function(exceldata)
    open($SUPWebSite)
    load_time
    login_as(exceldata)
    load_time
  end  


  #function of Open a supervisor web site.
  def open(url)
    #Maximize the browser.
    @browser.window.maximize   
    #Go to the URL web site. 
    @browser.goto(url)
    @browser.wait($TIMEOUT)
    self
  end

  #Login supervisor web site.
  def login_as(_data)
    input_data = _data
    if login_user_tb.exists?
      #verify login name whether includes domain. 
      if input_data[0] == "yes"
        #Input user name with domain.
        login_user_tb.set $_UserDomain+'\\'+input_data[1]
      else
        #Input user name without domain.
        login_user_tb.set input_data[1]
      end  
    else
      puts "error: Username textbox cannot be found on login page."
    end    

    if login_password_tb.exists?
      #Input password.
      login_password_tb.set input_data[2]
    else
      puts "error: Password textbox cannot be found on login page."
    end 

    login_log_bt.wait_until_present    
    if login_log_bt.enabled?      
      #Click on Login button.
      login_log_bt.click
    else
      puts "error: Log in button does not be enabled."
    end    
    @browser.wait($TIMEOUT)
  end

  #verify the error info is correct.
  def verifyError(info)
    if login_error_text.text == info
      return true
    else
      info = login_error_text.text
      puts "The warning message is: "+info  
      return false
    end
  end 
  
  #get client version from login page.
  def getClientVersion
    if login_client_version_text.exists?
      login_client_version_text.text
    else
      puts "error: It cannot find the version number in login page."
    end
  end  
  
  #get supervisor name from login page.
  def getSupervisorName
    if login_Supervisor_name_text.exists?
      login_Supervisor_name_text.text
    else
      puts "error: It cannot find the supervisor name in login page."
    end
  end   
  
  #get "Use your Windows NT account to log in." from login page.
  def getLoginLabel
    if login_title_label_text.exists?
      login_title_label_text.text
      return true
    else
      puts "error: It cannot find the 'Use your Windows NT account to log in.' in login page."
      return false
    end
  end   

  private

  #User Text Box.
  def login_user_tb
    @browser.text_field(:id => "UserName")
  end

  #Password Text Box.
  def login_password_tb
    @browser.text_field(:id => "Password")
  end

  #Login button.
  def login_log_bt
    @browser.button(:value => "Log in")
  end
  
  #Validation info.
  def login_error_text
    @browser.div(:id => "validationSummary")
  end
  
  #Client version.
  def login_client_version_text
    @browser.div(:id => "clientVersion")
  end
  
  #Supervisor name.
  def login_Supervisor_name_text
    @browser.div(:id => "loginContainer")
  end
  
  #Use your Windows NT account to log in.
  def login_title_label_text
    @browser.div(:id => "loginTitleLabel")
  end    
end

