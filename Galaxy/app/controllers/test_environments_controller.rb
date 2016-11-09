class TestEnvironmentsController < ApplicationController
  before_action :set_test_environment, only: [:show, :edit, :update, :destroy]

  # GET /test_environments
  # GET /test_environments.json
  def index
    @test_environments = TestEnvironment.get_all_force_test_environments
  end

  # GET /test_environments/1
  # GET /test_environments/1.json
  def show
    @test_environment = TestEnvironment.get_force_test_environment_by_id(params[:id])
  end

  # GET /test_environments/new
  def new
    @test_environment = TestEnvironment.new
  end

  # GET /test_environments/1/edit
  def edit
  end

  # POST /test_environments
  # POST /test_environments.json
  def create
    @test_environment = TestEnvironment.new(test_environment_params)
    @test_environment.created_at = DateTime.current
    respond_to do |format|
      if @test_environment.save
        format.html { redirect_to @test_environment, notice: 'Test environment was successfully created.' }
        format.json { render action: 'show', status: :created, location: @test_environment }
      else
        format.html { render action: 'new' }
        format.json { render json: @test_environment.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /test_environments/1
  # PATCH/PUT /test_environments/1.json
  def update
    respond_to do |format|
      @test_environment.modified_at = DateTime.current
      if @test_environment.update(test_environment_params)
        format.html { redirect_to @test_environment, notice: 'Test environment was successfully updated.' }
        format.json { head :no_content }
      else
        format.html { render action: 'edit' }
        format.json { render json: @test_environment.errors, status: :unprocessable_entity }
      end
    end
  end

  # DELETE /test_environments/1
  # DELETE /test_environments/1.json
  def destroy
    @test_environment.destroy
    respond_to do |format|
      format.html { redirect_to test_environments_url }
      format.json { head :no_content }
    end
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def set_test_environment
      @test_environment = TestEnvironment.find(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def test_environment_params
      params.require(:test_environment).permit(:name, :environment_type, :status, :created_at, :modified_at, :config, :description)
    end
end
