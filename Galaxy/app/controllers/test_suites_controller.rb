class TestSuitesController < ApplicationController
  before_action :set_test_suite, only: [:show, :edit, :update, :destroy]

  # GET /test_suites
  # GET /test_suites.json
  def index
    @test_suites = TestSuite.get_all_force_test_suites
  end

  # GET /test_suites/1
  # GET /test_suites/1.json
  def show
  end

  # GET /test_suites/new
  def new
    @test_suite = TestSuite.new
  end

  # GET /test_suites/1/edit
  def edit
    def @test_suite.new_record?()
      false
    end
  end

  # POST /test_suites
  # POST /test_suites.json
  def create
    test_suite_params['created_by'] = session[:user_id]
    @test_suite = TestSuite.create_force_test_suite(test_suite_params)
    respond_to do |format|
      if @test_suite
        format.html { redirect_to test_suite_path(@test_suite), notice: 'Test suite was successfully created.' }
        format.json { render action: 'show', status: :created, location: @test_suite }
      else
        format.html { render action: 'new' }
        format.json { render json: @test_suite.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /test_suites/1
  # PATCH/PUT /test_suites/1.json
  def update
    test_suite_params['modified_by'] = session[:user_id]
    test_suite = TestSuite.update_force_test_suite(params[:id], test_suite_params)
    respond_to do |format|
      if test_suite
        format.html { redirect_to test_suite_path(test_suite), notice: 'Test suite was successfully updated.' }
        format.json { head :no_content }
        format.js
      else
        format.html { render action: 'edit' }
        format.json { render json: @test_suite.errors, status: :unprocessable_entity }
        format.js
      end
    end
  end

  # DELETE /test_suites/1
  # DELETE /test_suites/1.json
  def destroy
    TestSuite.delete_force_test_suite(params[:id])
    respond_to do |format|
      format.html { redirect_to test_suites_url }
      format.json { head :no_content }
    end
  end


  def sub_test_suites    
    @test_suites = TestSuite.get_sub_test_suites_of_test_suite(params[:id]) 
  end
  
  def sub_test_cases
    @test_cases = TestCase.get_sub_test_cases_of_test_suite(params[:id])
  end
  
  def sub_test_suites_and_cases
    @test_suites = TestSuite.get_sub_test_suites_of_test_suite(params[:id])
    @test_cases = TestCase.get_sub_test_cases_of_test_suite(params[:id])
    if @test_suites != nil
      @test_suites.each do |suite|
        def suite.folder
          true
        end
        def suite.lazy
          true
        end
      end
    else
      @test_suites=[]
    end
    if @test_cases != nil
      @test_cases.each do |testcase|
        def testcase.folder
          false
        end
        def testcase.lazy
          false
        end
      end
    else
      @test_cases = []
    end
    @test_suites_and_cases = @test_suites + @test_cases
  end
  
  private
    # Use callbacks to share common setup or constraints between actions.
    def set_test_suite
      @test_suite = TestSuite.get_force_test_suite_by_id(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def test_suite_params
      params.require(:test_suite).permit(:name, :sub_suites, :test_cases, :created_by, :modified_by, :description, :is_root, :suite_type, :execution_command)
      #params[:test_suite][:sub_suites].delete('')
      #params[:test_suite][:sub_suites] = params[:test_suite][:sub_suites].join(',')
      #params[:test_suite][:test_cases].delete('')
      #params[:test_suite][:test_cases] = params[:test_suite][:test_cases].join(',')
    end
end
