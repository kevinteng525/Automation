require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
gem "minitest"
require "minitest/autorun"

#Login page==============================================================
class LoginPage < BrowserContainer

  #function of Open a web site.
  def open(url)
    #Maximize the browser.
    @browser.window.maximize   
    #Go to the URL web site. 
    @browser.goto(url)
    self
  end

  #Login Supervisor web site.
  def login_as(user, password)
  #Input user name.
  login_user_tb.set user

  #Input password.
  login_password_tb.set password

  #Click on Login button.
  login_log_bt.wait_until_present
  login_log_bt.click
  
  @browser.wait(20)
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
end 


