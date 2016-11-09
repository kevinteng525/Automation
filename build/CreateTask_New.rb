# ARGV 
#	0 Task Name 
#	1 ProductId 
#	2 ProjectId 
#	3 ReleaseId 
#	4 BranchId 
#	5 TestContent
#   6 EnvironmentId
#   7 EnableCodeCoverage Optional (0 or 1)
#   8 NotifyStakeholders Optional (0 or 1)
#   9 Username
require "rest_client"
require "json"
FORCE_SERVER = '10.98.28.194:8080'

def force_server
  RestClient::Resource.new(FORCE_SERVER, :headers=>{:accept => :json})
end
class WebTrigger
  def WebTrigger.refresh_build
    response = force_server['builds/refresh'].post "",:content_type =>'application/json'
    return response.body
  end
  def WebTrigger.latest_build(param)
    response = force_server["builds/latestbuild?productId=#{param[1]}&branchId=#{param[4]}&releaseId=#{param[3]}"].get :content_type =>'application/json'
    return response.body.gsub! /"/, ''
  end
  def WebTrigger.get_user_id(name)
    response = force_server["users/name/#{name}"].get :content_type =>'application/json'
    return response
  end
  def WebTrigger.create_task_with_latest_build(param)
	result = WebTrigger.refresh_build
	if param.size<10
	    user=0
	else	
	user = WebTrigger.get_user_id(param[9])
	puts user
	end
	if result.downcase=="true" and param.size>=7
	    build = WebTrigger.latest_build(param)
		task_param ={:Name=>"",:Status=>0,:Type=>0,:Priority=>0, :CreateDate=> "\/Date(#{Time.now.to_i}000)\/", :Information=>"",:ModifyDate=> "\/Date(#{Time.now.to_i}000)\/",:CreateBy=>user,:ModifyBy=>user,:BuildId=>0,:EnvironmentId=>"",:TestContent=>"",:Description=>"",:RecurrencePattern=>0,:StartDate=> "\/Date(#{Time.now.to_i}000)\/",:StartTime=> "\/Date(#{Time.now.to_i}000)\/",:WeekDays=>0,:WeekInterval=>1, :ParentTaskId=>0, :BranchId=>"", :ReleaseId=>"", :ProductId=>"", :ProjectId=>"", :NotifyStakeholders=>1, :WriteTestResultBack=>0, :SetupScript=>"", :TeardownScript=>"", :EnableCodeCoverage=>1 }
		task_param[:Name] = param[0]+"_"+build
		task_param[:ProductId] = param[1]
		task_param[:ProjectId] = param[2]
		task_param[:ReleaseId] = param[3]
		task_param[:BranchId] = param[4]
		task_param[:TestContent] = param[5]
		task_param[:EnvironmentId] = param[6]
		if param.size>=8
			task_param[:EnableCodeCoverage] = param[7]
		end
		if param.size>=9
			task_param[:NotifyStakeholders] = param[8]
		end
		force_server['tasks/'].post task_param.to_json,:content_type => 'application/json'
    else
    	puts "Not enough parameters"
	end
  end
end

WebTrigger.create_task_with_latest_build(ARGV)

